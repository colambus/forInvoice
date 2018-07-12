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
    [Route("api/TermsOfDelivery")]
    public class TermsOfDeliveryController : Controller
    {
        private readonly ApplicationContext _context;

        public TermsOfDeliveryController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet("[action]")]
        public IEnumerable<TermsOfDelivery> GetAll()
        {
            return _context.TermsOfDelivery.ToList();
        }

        [HttpPost("[action]")]
        public TermsOfDelivery Insert([FromBody]TermsOfDelivery entity)
        {
            _context.TermsOfDelivery.Add(entity);
            _context.SaveChanges();
            return entity;
        }
    }
}