using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;

using KvBackend.EFVersion;

namespace mvc_kv_backend.Controllers
{
    using System.Collections.Generic;
    using System.Web.OData;
    using System.Web.OData.Query;
    using System.Xml;

    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;

    //addl links:
    //1) open types - http://www.asp.net/web-api/overview/odata-support-in-aspnet-web-api/odata-v4/use-open-types-in-odata-v4
    //2) Customizing the query options - http://www.asp.net/web-api/overview/odata-support-in-aspnet-web-api/supporting-odata-query-options
    //3) Open complex types (Odata spec) - http://www.odata.org/getting-started/advanced-tutorial/
    // It is important to note that right now OData does *NOT* support unflat dynamic properties
    // -- Although 5.3 supports collections

    //maybe useful...
    //1) taking stuff from odata uri parser - http://blogs.msdn.com/b/odatateam/archive/2014/03/13/using-parameter-alias-to-simplify-the-odata-url.aspx
    //2) customizing the uri parser (and thus the resulting expression) - http://aspnet.codeplex.com/SourceControl/latest#Samples/WebApi/OData/v4/NHibernateQueryableSample/

    /*
    The WebApiConfig class may require additional changes to add a route for this controller. Merge these statements into the Register 
     * method of the WebApiConfig class as applicable. Note that OData URLs are case sensitive.

    using System.Web.Http.OData.Builder;
    using System.Web.Http.OData.Extensions;
    using KvBackend.EFVersion;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<RootObject>("RootObjects");
    builder.EntitySet<KvPairTable>("KvPairTables"); 
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */

    /// <summary>
    /// Important - There are a few critical items that must be implemented to support dynamic properties:
    /// 1) Dynamic prop must be IDictionary&lt;string,object&gt;
    /// 2) Dynamic props cannot be nested (only one level deep)
    /// 3) Dynamic props cannot contain restrict characters . , @ [ ] :
    /// </summary>
    public class RootObjectsController : ODataController
    {
        private StuffEntities db = new StuffEntities();

        private IEnumerable<RootObject> StaticData = new[]
        {
            new RootObject() { ActiveFlag = true, Description = "koo",Id = Guid.NewGuid(),OfferStartDate = DateTime.Now}, 
            new RootObject()
                {
                    ActiveFlag = false, Description = "oh yeah!",Id=Guid.NewGuid(),
                    ExtendedFields = new Dictionary<string, object>
                        {
                            {
                                "k1",
                                new Dictionary<string, object>
                                {
                                    {"k2","v2"}
                                }
                            }
                        }
                }
        };

        // GET: odata/RootObjects
        //[EnableQuery]
        //public IQueryable<RootObject> GetRootObjects()
        //{
        //    return db.RootObjects;
        //}

        // GET: odata/RootObjects
        public IQueryable<RootObject> Get(ODataQueryOptions opts)
        {
            var settings = new ODataValidationSettings()
            {
                // Initialize settings as needed.
                AllowedFunctions = AllowedFunctions.All
            };

            //gives Validating OData QueryNode of kind SingleValueOpenPropertyAccess is not supported by FilterQueryValidator (for dyn props)
            //also blows up when attempting to validate chars outside of the allowed preset (see RootObjectExtension for normalization)
            //opts.Validate(settings);
            //opts.Filter.FilterClause.Expression.Accept()//maybe pass a visitor?
            IQueryable results = opts.ApplyTo(db.RootObjects);

            var exp = opts.Filter.FilterClause.Expression;
            exp.

            //without customization, above will throw:
            //Additional information: Binding OData QueryNode of kind SingleValueOpenPropertyAccess is not supported by FilterBinder.
            //SEE: opts.Filter.FilterClause.Expression.Left.Source.Kind = SingleValueOpenPropertyAccess


            var tmpRes = results.Cast<RootObject>().ToList();
            return results as IQueryable<RootObject>;

            //return StaticData.AsQueryable();
        }

        // GET: odata/RootObjects(5)
        [EnableQuery]
        public SingleResult<RootObject> GetRootObject([FromODataUri] Guid key)
        {
            return SingleResult.Create(db.RootObjects.Where(rootObject => rootObject.Id == key));
        }


    //    private void ODataUrIPArsing()
    //    {
    //        Uri fullUri = new Uri("Products?$select=ID&$expand=ProductDetail" +
    //"&$filter=Categories/any(d:d/ID%20gt%201)&$orderby=ID%20desc" +
    //"&$top=1&$count=true&$search=tom", UriKind.Relative);
    //        //ODataUriParser parser = new ODataUriParser(model, serviceRoot, fullUri);
    //        Uri serviceRoot = new Uri("http://services.odata.org/V4/OData/OData.svc");
    //        IEdmModel model = EdmxReader.Parse(XmlReader.Create(serviceRoot + "/$metadata"));
    //        //Uri fullUri = new Uri("http://services.odata.org/V4/OData/OData.svc/Products");

    //        ODataUriParser parser = new ODataUriParser(model, serviceRoot, fullUri);
    //       //ODataQuerySettings qSet = new ODataQuerySettings{};
    //        //ODataQueryOptions qOp = new ODataQueryOptions()Filter=new FilterQueryOption(null,null,)};
    //        //ODataQueryOptionParser fff = new ODataQueryOptionParser(){};
    //        //ODataUriParserSettings pSet =new ODataUriParserSettings{}

    //        SelectExpandClause expand =
    //            parser.ParseSelectAndExpand();              //parse $select, $expand
    //        FilterClause filter = parser.ParseFilter();     // parse $filter
    //        OrderByClause orderby = parser.ParseOrderBy();  // parse $orderby
    //        SearchClause search = parser.ParseSearch();     // parse $search
    //        long? top = parser.ParseTop();                  // parse $top
    //        long? skip = parser.ParseSkip();                // parse $skip
    //        bool? count = parser.ParseCount();              // parse $count
    //    }







        // PUT: odata/RootObjects(5)
        public IHttpActionResult Put([FromODataUri] Guid key, Delta<RootObject> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            RootObject rootObject = db.RootObjects.Find(key);
            if (rootObject == null)
            {
                return NotFound();
            }

            patch.Put(rootObject);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RootObjectExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(rootObject);
        }

        // POST: odata/RootObjects
        public IHttpActionResult Post(RootObject rootObject)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.RootObjects.Add(rootObject);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (RootObjectExists(rootObject.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Created(rootObject);
        }

        // PATCH: odata/RootObjects(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] Guid key, Delta<RootObject> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            RootObject rootObject = db.RootObjects.Find(key);
            if (rootObject == null)
            {
                return NotFound();
            }

            patch.Patch(rootObject);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RootObjectExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(rootObject);
        }

        // DELETE: odata/RootObjects(5)
        public IHttpActionResult Delete([FromODataUri] Guid key)
        {
            RootObject rootObject = db.RootObjects.Find(key);
            if (rootObject == null)
            {
                return NotFound();
            }

            db.RootObjects.Remove(rootObject);
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/RootObjects(5)/KvPairTables
        [EnableQuery]
        public IQueryable<KvPairTable> GetKvPairTables([FromODataUri] Guid key)
        {
            return db.RootObjects.Where(m => m.Id == key).SelectMany(m => m.KvPairTables);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RootObjectExists(Guid key)
        {
            return db.RootObjects.Count(e => e.Id == key) > 0;
        }
    }
}
