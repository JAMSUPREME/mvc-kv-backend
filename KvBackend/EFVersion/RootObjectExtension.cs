using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KvBackend.EFVersion
{
    using System.Dynamic;
    using System.Text.RegularExpressions;

    public enum ExtendedFieldRetrieval
    {
        Flat = 1,
        Unflat = 2
    }
    partial class RootObject
    {
        private ExpandoObject _extendedFields;
        /// <summary>
        /// note: Might actually be faster using json.net to parse and unflatten the dictionary (couldn't find docs on it though)
        /// </summary>
        public dynamic ExtendedFields
        {
            get
            {
                if (_extendedFields == null)
                    throw new InvalidOperationException("First fill with FillExtendedFields (to demo both retrieval types)");
                return this._extendedFields;
            }
        }

        public void FillExtendedFields(ExtendedFieldRetrieval retrievalType)
        {
            switch (retrievalType)
            {
                case ExtendedFieldRetrieval.Flat:
                    _extendedFields = null;
                    this.FillExtendedFields();
                    break;
                case ExtendedFieldRetrieval.Unflat:
                    _extendedFields = null;
                    this.FillExtendedFieldsFancy();
                    break;
            }
        }

        /// <summary>
        /// does not handle fancy stuff. Basically a flat dictionary and anything fancier than a single field must be accessed via 
        /// full qualification, i.e. ((IDictionary&lt;string, object&gt;)first.ExtendedFields)["Cat[0].Name"]
        /// </summary>
        private void FillExtendedFields()
        {
            _extendedFields = new ExpandoObject();
            foreach (KvPairTable kvPair in KvPairTables)
            {
                ((IDictionary<string, object>)_extendedFields).Add(kvPair.Key, kvPair.Value);
            }
        }
        /// <summary>
        /// slightly fancier, handling indexers and deep objects
        /// </summary>
        private void FillExtendedFieldsFancy()
        {
            _extendedFields = new ExpandoObject();
            var extendedFields = _extendedFields as IDictionary<string, object>;
            if (extendedFields == null)
                throw new InvalidOperationException();

            foreach (KvPairTable kvPair in KvPairTables)
            {
                string[] deepProperties = kvPair.Key.Split('.');

                if (deepProperties.Length == 1)
                    extendedFields.Add(kvPair.Key, kvPair.Value);
                else
                {
                    if (deepProperties.Length > 1)
                    {
                        string[] indexers = Regex.Split(kvPair.Key, "\\[+\\d+\\]+.{1}");
                        if (indexers.Length == 1)
                        {
                            FillDeepFields(extendedFields, deepProperties, kvPair.Value);
                        }
                        else
                        {
                            //note: haven't really supported deep indexing yet, but not sure if we really need it..?
                            //current indexerIndex will just grab the first number it gets
                            string indexerIndex = Regex.Match(kvPair.Key, "\\[+\\d+\\]+").Value.Trim(new[] { '[', ']' });
                            this.FillIndexers(extendedFields, indexers[0], Convert.ToInt32(indexerIndex), indexers.Skip(1).ToArray(), kvPair.Value);
                        }
                    }
                }
            }
        }

        private void FillIndexers(IDictionary<string, object> dict, string indexerKey, int index, string[] qualifiedSplitFieldName, object value)
        {
            IDictionary<int, object> childDict;
            if (dict.ContainsKey(indexerKey))
            {
                //add to it
                childDict = dict[indexerKey] as IDictionary<int, object>;
            }
            else
            {
                //make it
                childDict = new Dictionary<int, object>();
                dict.Add(indexerKey, childDict);
            }
            if (childDict == null)
                throw new InvalidOperationException();

            IDictionary<string, object> childDictContent;
            if (childDict.ContainsKey(index))
            {
                childDictContent = childDict[index] as IDictionary<string, object>;
                if (childDictContent == null)
                    throw new InvalidOperationException();
            }
            else
            {
                childDictContent = new ExpandoObject();
                childDict.Add(index, childDictContent);
            }


            this.FillDeepFields(childDictContent, qualifiedSplitFieldName, value);
        }

        private void FillDeepFields(IDictionary<string, object> dict, string[] qualifiedSplitFieldName, object value)
        {
            if (qualifiedSplitFieldName.Length == 0)
                throw new InvalidOperationException();

            bool isDeepestChild = qualifiedSplitFieldName.Length == 1;

            if (isDeepestChild)
            {
                var key = qualifiedSplitFieldName[0];
                if (dict.ContainsKey(key))
                    dict[key] = value;
                else
                    dict.Add(key, value);
            }
            else
            {
                IDictionary<string, object> childDict = new ExpandoObject();
                this.FillDeepFields(childDict, qualifiedSplitFieldName.Skip(1).ToArray(), value);
                dict.Add(qualifiedSplitFieldName[0], childDict);
            }

        }
    }
}
