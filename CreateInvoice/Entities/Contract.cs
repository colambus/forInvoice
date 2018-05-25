using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CreateInvoice.Entities
{
    public class Contract: INamed
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual Organization Buyer { get; set; }
        public virtual Organization Seller { get; set; }
    }
}
