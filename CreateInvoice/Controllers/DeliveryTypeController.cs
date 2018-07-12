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
    [Route("api/DeliveryType")]
    public class DeliveryTypeController : Controller
    {
        private readonly ApplicationContext _context;

        public DeliveryTypeController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet("[action]")]
        public IEnumerable<DeliveryType> GetAll()
        {
            return _context.DeliveryTypes.ToList();
        }

        [HttpPost("[action]")]
        public DeliveryType Insert([FromBody]DeliveryType entity)
        {
            _context.DeliveryTypes.Add(entity);
            _context.SaveChanges();
            return entity;
        }
    }
}