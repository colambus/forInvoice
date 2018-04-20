using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CreateInvoice.Entities;
using CreateInvoice.Helpers;
using CreateInvoice.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CreateInvoice.Controllers
{
    [Produces("application/json")]
    [Route("api/Contract")]
    public class ContractController : Controller
    {
        private readonly ApplicationContext _context;

        public ContractController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet("[action]")]
        public IEnumerable<NamedModel> GetAll()
        {
            return _context.Contracts.ToList<INamed>().ToNamed();
        }
    }
}