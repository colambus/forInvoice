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
    }
}