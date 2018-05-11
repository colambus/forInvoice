using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CreateInvoice.Entities
{
    public class Invoice:IHaveId
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string InvoiceNo { get; set; }
        public Organization Seller { get; set; }
        public Organization Buyer { get; set; }
        public Contract Contract { get; set; }
        public DeliveryType DeliveryType { get; set; }
        public TermsOfDelivery TermsOfDelivery { get; set; }
        public TermOfPayment TermOfPayment { get; set; }
        public string PaymentIdentification { get; set; }
        public string OrderNo { get; set; }
    }
}
