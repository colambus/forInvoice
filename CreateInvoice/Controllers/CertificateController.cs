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
    [Route("api/Certificate")]
    public class CertificateController : Controller
    {
        private readonly ApplicationContext _context;

        public CertificateController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet("[action]")]
        public IEnumerable<Certificate> GetAll()
        {
            return _context.Certificates;
        }

        [HttpPut("[action]")]
        public Certificate Add([FromBody]Certificate certificate)
        {
            Certificate newCertificate = new Certificate()
            {
                Name = certificate.Name,
                StartDate = certificate.StartDate,
                EndDate = certificate.EndDate
            };

            _context.Certificates.Add(newCertificate);
            _context.SaveChanges();
            return newCertificate;
        }

        [HttpPost("[action]")]
        public Certificate Save([FromBody]Certificate certificate)
        {
            Certificate currCertificate = _context.Certificates.GetById(certificate.Id);
            if (currCertificate != null)
            {
                currCertificate.Name = certificate.Name;
                currCertificate.StartDate = certificate.StartDate;
                currCertificate.EndDate = certificate.EndDate;
                _context.SaveChanges();
            }

            return currCertificate;
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            Certificate certificate = _context.Certificates.FirstOrDefault(x => x.Id == id);
            if (certificate != null)
            {
                _context.Certificates.Remove(certificate);
                _context.SaveChanges();
            }
            return Ok(certificate);
        }
    }
}