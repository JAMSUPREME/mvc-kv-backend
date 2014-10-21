using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KvBackend.Tests
{
    using KvBackend.EFVersion;

    using NUnit.Framework;

    [TestFixture]
    class EnsureEFFill
    {
        [Test]
        public void GetOffers_EnsureFill()
        {
            using (var db = new StuffEntities())
            {
                var roots = from r in db.RootObjects 
                            select r;
                foreach (var root in roots)
                {
                    this.PrintRootOb(root);
                }
            }
        }
        [Test]
        public void GetSingleOffer_EnsureExtendedFields()
        {
            using (var db = new StuffEntities())
            {
                var first = (from r in db.RootObjects
                            select r).First();

                first.FillExtendedFields(ExtendedFieldRetrieval.Flat);
                var weirdPropName = ((IDictionary<string, object>)first.ExtendedFields)["Cat[0].Name"];
                var normalPropName = first.ExtendedFields.Name;
                var deepPropName = ((IDictionary<string, object>)first.ExtendedFields)["Hobby.Category.Name"];//first.ExtendedFields.Hobby.Category.Name;

                Console.WriteLine("---Flat---");

                Console.WriteLine(weirdPropName);
                Console.WriteLine(normalPropName);
                Console.WriteLine(deepPropName);

                first.FillExtendedFields(ExtendedFieldRetrieval.Unflat);
                weirdPropName = first.ExtendedFields.Cat[0].Name;
                normalPropName = first.ExtendedFields.Name;
                deepPropName = first.ExtendedFields.Hobby.Category.Name;

                Console.WriteLine();
                Console.WriteLine("---Unflat---");
                Console.WriteLine(weirdPropName);
                Console.WriteLine(normalPropName);
                Console.WriteLine(deepPropName);

            }
        }


        private void PrintRootOb(RootObject o)
        {
            Console.WriteLine("Offer: " + o.Id + " - Is active? " + (o.IsActive.Value ? "yes" : "no"));
        }
    }
}
