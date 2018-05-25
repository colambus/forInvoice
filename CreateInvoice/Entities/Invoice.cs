using System;
using CreateInvoice.Helpers;

namespace CreateInvoice.Entities
{
    public class Invoice:IHaveId
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string InvoiceNo { get; set; }
        public virtual Organization Seller { get; set; }
        public virtual Organization Buyer { get; set; }
        public virtual Contract Contract { get; set; }
        public virtual DeliveryType DeliveryType { get; set; }
        public virtual TermsOfDelivery TermsOfDelivery { get; set; }
        public virtual TermOfPayment TermOfPayment { get; set; }
        public string PaymentIdentification { get; set; }
        public string OrderNo { get; set; }
    }
}
