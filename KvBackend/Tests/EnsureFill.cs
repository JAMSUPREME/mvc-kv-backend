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
        [Test]
        public void GetOffers_EnsureFill()
        {
            //todo
            KVRepository repo = new KVRepository(ConfigurationManager.ConnectionStrings["StuffDb"].ConnectionString);

            var offers = repo.GetOffers();

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
