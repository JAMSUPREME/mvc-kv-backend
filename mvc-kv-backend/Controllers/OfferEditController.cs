using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KvBackend;

namespace mvc_kv_backend.Controllers
{
    public class OfferEditController : Controller
    {
        // GET: OfferEdit
        [HttpGet]
        public ActionResult Index()
        {
            var offer = new KVRepository().GetOffers().First(o => o.Id.Equals(new Guid("DE3AA882-CC88-456A-A2CB-155D477D00A2")));
            return View(offer);
        }

        [HttpPost]
        public ActionResult Index([ModelBinder(typeof(OfferModelBinder))] Offer o)
        {
            //imagine we save
            new KVRepository().UpdateOffer(o);
            return this.View(o);
        }
    }
}