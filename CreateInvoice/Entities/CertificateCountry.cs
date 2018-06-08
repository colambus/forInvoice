using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CreateInvoice.Entities
{
    public class CertificateCountry
    {
        [Key]
        public int CertificateId { get; set; }
        [Key]
        public int CountryId { get; set; }
        public virtual Certificate Certificate { get; set; }
        public virtual Country Country { get; set; }
    }
}
