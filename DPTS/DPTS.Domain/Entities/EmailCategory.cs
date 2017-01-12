using System.Collections.Generic;

namespace DPTS.Domain.Entities
{
    public partial class EmailCategory :BaseEntityWithDateTime
    {
        public EmailCategory()
        { 
        }

        public string Name { get; set; } 

        public bool Published { get; set; }

        public int DisplayOrder { get; set; }
         
    }
}
