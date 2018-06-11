using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Web;
using System.Threading.Tasks;
using CreateInvoice.Entities;
using CreateInvoice.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Net.Http.Headers;
using CreateInvoice.ViewModel;

namespace CreateInvoice.Controllers
{
    [Produces("application/json")]
    [Route("api/Certificate")]
    public class CertificateController : Controller
    {
        private readonly ApplicationContext _context;
        private IHostingEnvironment _hostingEnvironment;

        public CertificateController(ApplicationContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet("[action]")]
        public IEnumerable<Certificate> GetAll()
        {
            return _context.Certificates;
        }

        [HttpPut("[action]")]
        public Certificate Add([FromBody]Certificate certificate)
        {
            if (certificate == null)
                return certificate;

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
            if (certificate != null)
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
            return certificate;
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            Certificate certificate = _context.Certificates.FirstOrDefault(x => x.Id == id);
            try
            {
                if (certificate != null)
                {
                    _context.Certificates.Remove(certificate);
                    _context.SaveChanges();
                }
                return Ok(certificate);
            }
            catch
            {
                return View("Error");
            }
        }


        [HttpPost("[action]")]
        [DisableRequestSizeLimit]
        public IActionResult Upload()
        {
            try
            {              
                var file = Request.Form.Files[0];
                if (file.Length > 0)
                {
                    List<Certificate> certificates = ConvertHelper.CertificatesFromXLSToList(file.OpenReadStream());
                    if (certificates.Count() == 0)
                    {
                        return BadRequest();
                    }
                    foreach (var el in certificates)
                    {
                        if (!_context.Certificates.Any(p => p.Name == el.Name))
                            _context.Certificates.Add(el);
                    }
                    _context.SaveChanges();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return Json("Upload Failed: " + ex.Message);
            }

        }

        private CertificateDTO ModelToDTO(Certificate certificate)
        {
            return new CertificateDTO
            {
                Id = certificate.Id,
                EndDate = certificate.EndDate,
                StartDate = certificate.StartDate,
                Name = certificate.Name
            };
        }

        private Certificate DTOtoModel(CertificateDTO certificate)
        {
            if (certificate == null) return null;
            return new Certificate
            {
                Id = certificate.Id,
                EndDate = certificate.EndDate,
                StartDate = certificate.StartDate,
                Name = certificate.Name
            };
        }

        [HttpGet("[action]")]
        public IActionResult DownloadTemplate()
        {
            var locatedFile = System.IO.File.OpenRead(@"C:\Users\eovcharenko\source\repos\CreateInvoice\CreateInvoice\Templates\Certificate import template.xlsx");
            var response = File(locatedFile, "application/octet-stream");

            return response;
        }
    }
}