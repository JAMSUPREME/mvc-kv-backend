using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KvBackend.Tests
{
    using NUnit.Framework;

    [TestFixture]
    class EnsureFilter
    {
        [Test]
        public void TestAllFilters()
        {
            KVRepository repo = new KVRepository();

            var offers = repo.GetOffers();
            //assume we also hold onto our extended fields!
            var extFields = offers.First().ExtendedFields.Keys.ToArray();

            var filteredOffers = repo.GetOffers(wildCardEquality: new Tuple<string, string[]>("whisk", extFields));

            Assert.That(filteredOffers.Count() > 0, "Expected something to match the filter");
        }
    }
}
