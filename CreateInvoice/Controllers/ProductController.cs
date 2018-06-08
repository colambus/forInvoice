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
    [Route("api/Product")]
    public class ProductController : Controller
    {
        private readonly ApplicationContext _context;

        public ProductController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet("[action]")]
        public IEnumerable<Product> GetBySeq([FromQuery]string queryStr)
        {
            return _context.Products
                .Include("CountryOfOrigin")
                .Where(p=>p.CodeNo.Contains(queryStr))
                .ToList();
        }

        [HttpGet("[action]")]
        public IEnumerable<Product> GetAll([FromQuery]string skip, [FromQuery]string take)
        {
            var result = _context.Products
                .Include(p => p.CountryOfOrigin)
                .Include(p => p.Certificate)
                .Include(p => p.CountryOfOrigin);

            if (skip != null)
                result.Skip(int.Parse(skip));
            if (take != null)
                result.Take(int.Parse(take));

            return result;
        }

        [HttpPut("[action]")]
        public Product Add([FromBody]Product product)
        {
            Product newProduct = new Product()
            {
                CodeNo = product?.CodeNo,
                DescriptionEn = product?.DescriptionEn,
                DescriptionUa = product?.DescriptionUa,
                Certificate = _context.Certificates.GetById(product?.Certificate?.Id),
                CountryOfOrigin = _context.Countries.GetById(product?.CountryOfOrigin?.Id)
            };

            _context.Products.Add(newProduct);
            _context.SaveChanges();
            return newProduct;
        }

        [HttpPost("[action]")]
        public Product Save([FromBody]Product product)
        {
            Product currProduct = _context.Products.GetById(product.Id);
            if (currProduct != null)
            {
                currProduct.DescriptionEn = product.DescriptionEn;
                currProduct.DescriptionUa = product.DescriptionUa;
                currProduct.CodeNo = product.CodeNo;
                currProduct.Certificate = _context.Certificates.GetById(product.Certificate?.Id);
                currProduct.CountryOfOrigin = _context.Countries.GetById(product.CountryOfOrigin?.Id);
                _context.SaveChanges();
            }

            return currProduct;
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            Product product = _context.Products.FirstOrDefault(x => x.Id == id);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
            return Ok(product);
        }
    }
}