using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CreateInvoice.Entities;
using CreateInvoice.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
            Invoice invoice = new Invoice();
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
    }
}