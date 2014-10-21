using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KvBackend.Tests
{
    using System.Data.Entity.Core.Objects;
    using System.Data.Entity.Infrastructure;

    using KvBackend.EFVersion;

    using NUnit.Framework;

    [TestFixture]
    class EnsureEFFilter
    {
        [Test]
        public void TestAllFilters()
        {
            //our desired query:
            //SELECT
            //Id,
            //ROW_NUMBER() OVER (ORDER BY Id) as RowNum
            //FROM RootObject r
            //WHERE r.Id IN (
            //    SELECT RootObjectId
            //    FROM KvPairTable
            //    WHERE [Key] = 'Cat[0].Name'
            //        AND Value LIKE '%whi%'
            //)

            using (var db = new StuffEntities())
            {
                //var roots = (from r in db.RootObjects
                //             join kv in db.KvPairTables on r.Id equals kv.RootObjectId
                //             select r).OrderBy(r => r.Id).Skip(5).Take(5);
                var roots = (from rOuter in db.RootObjects
                                 .Include("KvPairTables")
                             where rOuter.KvPairTables.Any(kv => kv.Key == "Cat[0].Name" && kv.Value.Contains("whi"))
                             select rOuter
                                 );
                Console.WriteLine(roots.ToString());
            }
            //KVRepository repo = new KVRepository();

            //var offers = repo.GetOffers();
            ////assume we also hold onto our extended fields!
            //var extFields = offers.First().ExtendedFields.Keys.ToArray();

            //var filteredOffers = repo.GetOffers(wildCardEquality: new Tuple<string, string[]>("whisk", extFields));

            //Assert.That(filteredOffers.Count() > 0, "Expected something to match the filter");
        }
    }
}
