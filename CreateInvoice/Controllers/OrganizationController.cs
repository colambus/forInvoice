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
    [Route("api/Organization")]
    public class OrganizationController : Controller
    {
        private readonly ApplicationContext _context;

        public OrganizationController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet("[action]")]
        public IEnumerable<Organization> GetAll()
        {
            return _context.Organizations.ToList();
        }

        [HttpPost]
        public Organization Insert([FromBody]Organization entity)
        {
            _context.Organizations.Add(entity);
            return entity;
        }
    }
}