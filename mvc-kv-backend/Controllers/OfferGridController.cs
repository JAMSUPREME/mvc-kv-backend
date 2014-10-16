using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KvBackend;

namespace mvc_kv_backend.Controllers
{
    public class OfferGridController : Controller
    {
        // GET: OfferGrid
        public ActionResult Index()
        {
            return View(new KVRepository().GetOffers());
        }
    }
}