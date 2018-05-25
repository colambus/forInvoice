using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CreateInvoice.Entities;
using CreateInvoice.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreateInvoice.Controllers
{
    [Produces("application/json")]
    [Route("api/InvoiceProduct")]
    public class InvoiceProductController : Controller
    {
        private readonly ApplicationContext _context;

        public InvoiceProductController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet("[action]")]
        public IEnumerable<InvoiceProducts> GetAll(int id)
        {
            return _context.InvoiceProducts
                .Include(p => p.Invoice)
                .Include(p => p.Product)
                .ThenInclude(p => p.CountryOfOrigin)
                .ThenInclude(p => p.Certificate)
                .Where(p => p.Invoice.Id == id)
                .ToList();
        }

        [HttpPut("[action]")]
        public InvoiceProducts Add([FromBody]InvoiceProducts product)
        {
            Invoice incoice = _context.Invoices.GetById(product.Invoice?.Id);
            product.ProductPosition = _context.InvoiceProducts
                .Where(i => i.Invoice == product.Invoice)
                .Count() + 1;

            product.Invoice = _context.Invoices.GetById(product.Invoice?.Id);
            product.Product = _context.Products.GetById(product.Product?.Id);

            _context.InvoiceProducts.Add(product);
            _context.SaveChanges();

            return product;
        }

    }
}