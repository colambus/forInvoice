﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CreateInvoice.Entities
{
    public class DeliveryType: INamed
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}