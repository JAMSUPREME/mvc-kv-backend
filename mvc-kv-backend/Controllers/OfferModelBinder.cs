using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mvc_kv_backend.Controllers
{
    using System.ComponentModel;
    using System.Web.Mvc;

    public class OfferModelBinder : DefaultModelBinder
    {
        protected override void BindProperty(
            ControllerContext controllerContext,
            ModelBindingContext bindingContext,
            PropertyDescriptor propertyDescriptor)
        {
            if (propertyDescriptor.DisplayName.Equals("ExtendedFields", StringComparison.OrdinalIgnoreCase))
            {
                var form = controllerContext.HttpContext.Request.Form;
                var extFieldProp = propertyDescriptor.GetValue(bindingContext.Model) as Dictionary<string, object>;
                if (extFieldProp == null)
                    return;
                foreach (var key in form.AllKeys.Where(k => k.StartsWith("ExtendedFields", StringComparison.OrdinalIgnoreCase)))
                {
                    //bind prop
                    var strippedKey = this.StripDictIndexer(key);
                    extFieldProp.Add(strippedKey, form[key]);
                }
            }
            else
                base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
        }

        private string StripDictIndexer(string origKey)
        {
            return origKey.Substring(15, origKey.Length - 16);
        }
    }
}