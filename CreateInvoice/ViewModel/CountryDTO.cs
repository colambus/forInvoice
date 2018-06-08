using CreateInvoice.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CreateInvoice.ViewModel
{
    public class CountryDTO
    {
        public string Name { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionUa { get; set; }
        public int CountryId { get; set; }
        public int CertificateId { get; set; }
    }
}
