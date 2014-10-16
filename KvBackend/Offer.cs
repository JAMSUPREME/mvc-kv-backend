using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KvBackend
{
    /// <summary>
    /// filling logic could either go here or the repository?
    /// </summary>
    public class Offer
    {
        public Offer(bool isActive)
        {
            this.IsActive = isActive;
            ExtendedFields = new Dictionary<string, object>();
        }

        public Guid Id { get; set; }
        public string Description { get; set; }
        public DateTime? OfferStartDate { get; set; }
        public DateTime? OfferEndDate { get; set; }
        public bool? ActiveFlag { get; set; }
        /// <summary>
        /// computed in db
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        /// Right now extended fields are limited to explicit varchar 
        /// unless we want to add a datatype
        /// column into the model so we know what it should map into
        /// </summary>
        public Dictionary<string, object> ExtendedFields { get; set; } 
    }
}
