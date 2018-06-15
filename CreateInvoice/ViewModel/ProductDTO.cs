using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CreateInvoice.ViewModel
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionUa { get; set; }
        public string CodeNo { get; set; }
        public DateTime? CertificateStartDate { get; set; }
        public DateTime? CertificateEndDate { get; set; }
        public string CertificateName { get; set; }
        public string CountryDescriptionEn { get; set; }
        public string CountryName { get; set; }
    }
}
