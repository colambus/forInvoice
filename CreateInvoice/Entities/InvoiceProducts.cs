using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CreateInvoice.Entities
{
    public class InvoiceProducts:IHaveId
    {
        public int Id { get; set; }
        public Invoice Invoice { get; set; }
        public int ProductPosition { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
    }
}
