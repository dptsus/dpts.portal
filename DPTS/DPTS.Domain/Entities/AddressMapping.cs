using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPTS.Domain.Entities
{
    public class AddressMapping :BaseEntity
    {
        public string UserId { get; set; }

        public int AddressId { get; set; }
    }
}
