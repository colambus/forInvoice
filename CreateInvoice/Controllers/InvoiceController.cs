using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CreateInvoice.Entities;
using CreateInvoice.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CreateInvoice.Controllers
{
    [Produces("application/json")]
    [Route("api/Invoice")]
    public class InvoiceController : Controller
    {
        private readonly ApplicationContext _context;

        public InvoiceController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet("[action]")]
        public IEnumerable<Invoice> GetAll()
        {
            return _context.Invoices
                .ToList();
        }

        [HttpGet("[action]")]
        public Invoice CreateNewInvoice()
        {
            Invoice invoice = new Invoice
            {
                Date = DateTime.Now,
                InvoiceNo = InvoiceHelper.CreateNewNo(DateTime.Now, GetInvoiceCount(DateTime.Now))
            };
            _context.Invoices.Add(invoice);
            _context.SaveChanges();

            return invoice;
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            Invoice invoice = _context.Invoices.FirstOrDefault(x => x.Id == id);
            if (invoice != null)
            {
                _context.Invoices.Remove(invoice);
                _context.SaveChanges();
            }
            return Ok(invoice);
        }

        [HttpPost("[action]")]
        public Invoice GetById([FromBody]Invoice invoice)
        {
            Invoice result = _context.Invoices
                .Include(p=>p.Buyer)
                .Include(p=>p.Contract)
                .Include(p=>p.DeliveryType)
                .Include(p=>p.Seller)
                .Include(p=>p.TermOfPayment)
                .Include(p=>p.TermsOfDelivery)
                .FirstOrDefault(x => x.Id == invoice.Id);

            return result;
        }

        [HttpPut("[action]")]
        public Invoice Save([FromBody]Invoice invoice)
        {
            Invoice currInvoice = _context.Invoices.Where(i => i.Id == invoice.Id).FirstOrDefault();
            SetNewValues(ref currInvoice, invoice);

            currInvoice.InvoiceNo = InvoiceHelper.CreateNewNo(invoice.Date, GetInvoiceCount(invoice.Date));
            _context.SaveChanges();
            return invoice;
        }

        private int GetInvoiceCount(DateTime date)
        {
            var numberOfInvoices = _context.Invoices
                .Where(d => d.Date >= DateTime.Now.Date.Date && d.Date >= DateTime.Now.Date.AddDays(1).AddTicks(-1))
                .Count() + 1;

            return numberOfInvoices;
        }

        private Invoice SetNewValues(ref Invoice currInvoice, Invoice invoice)
        {
            currInvoice.Seller = _context.Organizations.GetById(invoice.Seller?.Id); 
            currInvoice.Buyer = _context.Organizations.GetById(invoice.Buyer?.Id);
            currInvoice.Contract = _context.Contracts.GetById(invoice.Contract?.Id);
            currInvoice.Date = invoice.Date;
            currInvoice.DeliveryType = _context.DeliveryTypes.GetById(invoice.DeliveryType?.Id);
            currInvoice.TermOfPayment = _context.TermsOfPayment.GetById(invoice.TermOfPayment?.Id);
            currInvoice.TermsOfDelivery = _context.TermsOfDelivery.GetById(invoice.TermOfPayment?.Id);

            return currInvoice;
        }
    }
}