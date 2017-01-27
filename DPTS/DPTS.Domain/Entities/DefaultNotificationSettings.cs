using System.Collections.Generic;

namespace DPTS.Domain.Entities
{
    public partial class DefaultNotificationSettings : BaseEntityWithDateTime
    {
        public DefaultNotificationSettings()
        { 
        }

        public int CategoryId { get; set; }

        public string Name { get; set; }

        public string Message { get; set; }

        public bool Published { get; set; }

        public int DisplayOrder { get; set; }
         

        public virtual EmailCategory EmailCategory { get; set; }
    }
}
