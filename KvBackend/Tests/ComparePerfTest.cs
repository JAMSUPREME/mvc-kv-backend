using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KvBackend.Tests
{
    using System.Data.Entity;
    using System.Diagnostics;

    using KvBackend.EFVersion;

    using NUnit.Framework;

    [TestFixture]
    class ComparePerfTest
    {
        [Test]
        public void TestSpeed()
        {

            Stopwatch s = new Stopwatch();
            using (var db = new StuffEntities())
            {
                var roots = (from rOuter in db.RootObjects
                             where rOuter.KvPairTables.Any(kv => kv.Key == "Cat[0].Name" && kv.Value.Contains("whisk"))
                             select rOuter
                                 );
                foreach (var r in roots)
                {
                    //bleh
                }
                //throwing out first op

                roots = (from rOuter in db.RootObjects
                         where rOuter.KvPairTables.Any(kv => kv.Key == "Cat[0].Name" && kv.Value.Contains("whisk"))
                         select rOuter
                                 );
                s.Start();
                foreach (var r in roots)
                {
                    Console.WriteLine(r.Id);
                }
                s.Stop();
            }
            Console.WriteLine("EF - " + s.ElapsedMilliseconds + "ms");
            s.Reset();

            s.Start();
            KVRepository repo = new KVRepository();
            var offers = repo.GetOffers();
            var extFields = offers.First().ExtendedFields.Keys.ToArray();
            var filteredOffers = repo.GetOffers(wildCardEquality: new Tuple<string, string[]>("whisk", extFields));
            foreach (var o in filteredOffers)
            {
                Console.WriteLine(o.Id);
            }
            s.Stop();
            Console.WriteLine("SQL pivot - " + s.ElapsedMilliseconds + "ms");
        }
    }
}
