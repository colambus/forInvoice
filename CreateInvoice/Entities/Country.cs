using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CreateInvoice.Entities
{
    public class Country: IHaveId
    {
        public int Id { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionUa { get; set; }
        public Certificate Certificate { get; set; }
    }
}
