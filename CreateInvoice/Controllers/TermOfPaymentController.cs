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
    [Route("api/TermOfPayment")]
    public class TermOfPaymentController : Controller
    {
        private readonly ApplicationContext _context;

        public TermOfPaymentController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet("[action]")]
        public IEnumerable<TermOfPayment> GetAll()
        {
            return _context.TermsOfPayment.ToList();
        }

        [HttpPost("[action]")]
        public TermOfPayment Insert([FromBody]TermOfPayment entity)
        {
            _context.TermsOfPayment.Add(entity);
            _context.SaveChanges();
            return entity;
        }
    }
}