using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KvBackend.EFVersion
{
    using System.Dynamic;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Xml.Schema;

    public enum ExtendedFieldRetrieval
    {
        Flat = 1,
        Unflat = 2,
        FlatWithKeyNormalization = 3,
    }
    partial class RootObject
    {
        public DateTimeOffset OfferStartDateTimeOffset
        {
            get
            {
                if (OfferStartDate.HasValue)
                    return new DateTimeOffset(OfferStartDate.Value);

                return new DateTimeOffset();
            }
        }
        public DateTimeOffset OfferEndDateTimeOffset
        {
            get
            {
                if (OfferEndDate.HasValue)
                    return new DateTimeOffset(OfferEndDate.Value);

                return new DateTimeOffset();
            }
        }

        private IDictionary<string, object> _extendedFields;//expando obj will be the concrete impl. for this
        /// <summary>
        /// note: Might actually be faster using json.net to parse and unflatten the dictionary (couldn't find docs on it though)
        /// Web API w/ OData doesn't support unflat extended (dynamic) properties
        /// </summary>
        public IDictionary<string, object> ExtendedFields
        {
            get
            {
                if (_extendedFields == null)
                    FillExtendedFields(ExtendedFieldRetrieval.FlatWithKeyNormalization);//default to flat (OData can't handle unflat without complicated code) - can easily be commented to resume invalid op behavior
                //throw new InvalidOperationException("First fill with FillExtendedFields (to demo both retrieval types)");
                return this._extendedFields;
            }
            set
            {
                _extendedFields = value;//IMPORTANT - REMOVE THIS SETTER    (ONLY EXISTS FOR STATIC CHECK)
            }
        }

        /// <summary>
        /// For now split between normal ExtendedFields and "dynamic" extended fields since the OData convention builder must find a property
        /// matching IDictionary&lt;string, object&gt; (not dynamic)
        /// Not a permanent concept, but for now the split will make life easier
        /// </summary>
        public dynamic ExtendedFieldsDynamic
        {
            get
            {
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
                case ExtendedFieldRetrieval.FlatWithKeyNormalization:
                    _extendedFields = null;
                    this.FillExtendedFields();

                    //var schema = "<xs:simpleType name=\"TSimpleIdentifier\">"
                    //    + "<xs:restriction base=\"xs:string\">"
                    //    + "<xs:maxLength value=\"480\" />"
                    //    + "<xs:pattern value=\"[\\p{L}\\p{Nl}][\\p{L}\\p{Nl}\\p{Nd}\\p{Mn}\\p{Mc}\\p{Pc}\\p{Cf}]{0,}\" />"
                    //    + "</xs:restriction>"
                    //    + "</xs:simpleType>";
                    var regex = "[\\p{L}\\p{Nl}][\\p{L}\\p{Nl}\\p{Nd}\\p{Mn}\\p{Mc}\\p{Pc}\\p{Cf}]{0,}";

                    //XmlSchema s = XmlSchema.Read(new StringReader(schema), (o, e) => { });

                    //normalize keys
                    IDictionary<string, object> newDict = new Dictionary<string, object>();
                    foreach (KeyValuePair<string, object> pair in _extendedFields)
                    {
                        var newKey = pair.Key.Replace('[', '_').Replace(']', '_').Replace('.', '_');//somewhat sloppy since indexer and props will have the same syntax (assumption that a singular number implies an indexer)
                        newDict.Add(newKey, pair.Value);
                    }
                    _extendedFields = newDict;
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
