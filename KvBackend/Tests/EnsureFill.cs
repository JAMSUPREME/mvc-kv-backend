using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace KvBackend.Tests
{
    /// <summary>
    /// makes sure fill works
    /// </summary>
    [TestFixture]
    class EnsureFill
    {
        [Test]
        public void ConnectionTest()
        {
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["StuffDb"].ConnectionString))
            {
                conn.Open();
            }
        }
        /// <summary>
        /// note: It is likely that joining the KV and then flattening it manually is a lot faster than pivoting, since the pivot requires 
        /// determining the specific fields that will be pulled out into columns
        /// </summary>
        [Test]
        public void GetOffers_EnsureFill()
        {
            KVRepository repo = new KVRepository();

            var offers = repo.GetOffers();

            foreach (Offer offer in offers)
            {
                PrintOffer(offer);
            }
        }
        [Test]
        public void GetOffers_EnsureFill_WithPaging()
        {
            KVRepository repo = new KVRepository();

            var offers = repo.GetOffers(pageStart: 1, pageEnd: 1).ToList();

            Assert.That(offers.Count() == 1);
            foreach (Offer offer in offers)
            {
                PrintOffer(offer);
            }
        }

        private void PrintOffer(Offer o)
        {
            Console.WriteLine("Offer: " + o.Id + " - Is active? " + (o.IsActive ? "yes" : "no"));
        }
    }
}
