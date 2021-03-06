﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CreateInvoice.Entities
{
    public class Certificate: INamed
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public ICollection<CertificateCountry> CertificateCountries { get; set; } = new List<CertificateCountry>();
    }
}
