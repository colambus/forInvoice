﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CreateInvoice.Entities
{
    public class Product: IHaveId
    {
        public int Id { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionUa { get; set; }
        public virtual Certificate Certificate { get; set; }
        public string CodeNo { get; set; }
        public virtual Country CountryOfOrigin { get; set; }
    }
}
