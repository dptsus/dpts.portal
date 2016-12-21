using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPTS.Domain.Entities
{
    public class Country : BaseEntityWithDateTime
    {
        public string Name { get; set; }

        public string TwoLetterIsoCode { get; set; }

        public string ThreeLetterIsoCode { get; set; }

        public int NumericIsoCode { get; set; }

        public bool SubjectToVat { get; set; }

        public bool Published { get; set; }

        public int DisplayOrder { get; set; }

    }
}
