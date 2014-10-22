using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace mvc_kv_backend
{
    using System.Web.OData.Builder;
    using System.Web.OData.Extensions;

    using KvBackend.EFVersion;

    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            var builder = new ODataConventionModelBuilder();
            BuildODataModel(builder);
            config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
        }

        private static void BuildODataModel(ODataConventionModelBuilder builder)
        {
            //convention based
            builder.EntitySet<RootObject>("RootObjects");

            var rootObjTypeDef = builder.EntityType<RootObject>();
            rootObjTypeDef.Ignore(r => r.OfferEndDate);
            rootObjTypeDef.Ignore(r => r.OfferStartDate);
            builder.EntitySet<KvPairTable>("KvPairTables");

            var kvPairObjTypeDef = builder.EntityType<KvPairTable>();
            kvPairObjTypeDef.HasKey(kv => kv.ObjectId);


            //explicit
            //ComplexTypeConfiguration<RootObject> rootObjType = builder.ComplexType<RootObject>();
            //rootObjType.Property(c => c.Id);
            //// ...
            //rootObjType.HasDynamicProperties(c => c.DynamicProperties);
            //rootObjType.

            //EntityTypeConfiguration<Book> bookType = builder.EntityType<Book>();
            //bookType.HasKey(c => c.ISBN);
            //bookType.Property(c => c.Title);
            //// ...
            //bookType.ComplexProperty(c => c.Press);
            //bookType.HasDynamicProperties(c => c.Properties);

            //// ...
            //builder.EntitySet<Book>("Books");
            //IEdmModel model = builder.GetEdmModel();
        }
    }
}
