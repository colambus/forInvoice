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
            if (_context.InvoiceProducts.Any(p => p.Product == product.Product && p.Invoice == product.Invoice))
            {
                var currProduct = _context.InvoiceProducts
                    .FirstOrDefault(p => p.Product == product.Product && p.Invoice == product.Invoice);
                currProduct.Quantity = currProduct.Quantity + product.Quantity;
            }
            else
            {
                product.ProductPosition = _context.InvoiceProducts
               .Where(i => i.Invoice == product.Invoice)
               .Count() + 1;
                product.Invoice = _context.Invoices.GetById(product.Invoice?.Id);
                product.Product = _context.Products.GetById(product.Product?.Id);
                _context.InvoiceProducts.Add(product);
            }

            _context.SaveChanges();

            return product;
        }

        [HttpPost("[action]")]
        public InvoiceProducts Save([FromBody]InvoiceProducts product)
        {
            Invoice incoice = _context.Invoices.GetById(product.Invoice?.Id);
            InvoiceProducts invoiceProduct = _context.InvoiceProducts.GetById(product?.Id);

            if (invoiceProduct != null)
            {
                invoiceProduct.Invoice = _context.Invoices.GetById(product.Invoice?.Id);
                invoiceProduct.Product = _context.Products.GetById(product.Product?.Id);
                invoiceProduct.Quantity = product.Quantity;
                invoiceProduct.UnitPrice = product.UnitPrice;

                _context.SaveChanges();
            }

            return invoiceProduct;
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            InvoiceProducts invoiceProduct = _context.InvoiceProducts.FirstOrDefault(x => x.Id == id);

            if (invoiceProduct != null)
            {
                int position = invoiceProduct.ProductPosition + 1;

                IEnumerable<InvoiceProducts> higherPorducts = _context.InvoiceProducts
                .Where(p => p.Invoice == invoiceProduct.Invoice && p.ProductPosition > position).DefaultIfEmpty();

                _context.InvoiceProducts.Remove(invoiceProduct);
                if (invoiceProduct.Invoice != null)
                    if (higherPorducts.Any())
                    {
                        higherPorducts.OrderBy(p => p.ProductPosition);
                        foreach (InvoiceProducts prod in higherPorducts)
                        {
                            prod.ProductPosition = position;
                            position++;
                        }
                    }

                _context.SaveChanges();
            }
            return Ok(invoiceProduct);
        }
    }
}