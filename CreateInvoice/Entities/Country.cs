using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CreateInvoice.Entities
{
    public class Country: IHaveId, INamed
    {
        public string DescriptionEn { get; set; }
        public string DescriptionUa { get; set; }
        public ICollection<CertificateCountry> CountryCertificates { get; set; } = new List<CertificateCountry>();
        public string Name { get; set; }
        public int Id { get; set; }
    }
}
