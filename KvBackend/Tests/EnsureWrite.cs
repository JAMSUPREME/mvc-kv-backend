using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KvBackend.Tests
{
    using System.Configuration;

    using NUnit.Framework;

    [TestFixture]
    class EnsureWrite
    {
        [Test]
        public void SaveOffer_EnsureKVWrite()
        {
            KVRepository repo = new KVRepository();
            var offer = repo.GetOffers().First(o => o.Id.Equals(new Guid("DE3AA882-CC88-456A-A2CB-155D477D00A2")));

            offer.ExtendedFields.Add("NewExtField", "BANANASSSS");

            repo.UpdateOffer(offer);

            offer = repo.GetOffers().First(o => o.Id.Equals(new Guid("DE3AA882-CC88-456A-A2CB-155D477D00A2")));

            Assert.That((string)offer.ExtendedFields["NewExtField"] == "BANANASSSS");
        }
    }
}
