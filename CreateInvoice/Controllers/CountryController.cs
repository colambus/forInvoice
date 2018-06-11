using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using CreateInvoice.Entities;
using CreateInvoice.Helpers;
using CreateInvoice.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreateInvoice.Controllers
{
    [Produces("application/json")]
    [Route("api/Country")]
    public class CountryController : Controller
    {
        private readonly ApplicationContext _context;

        public CountryController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet("[action]")]
        public IEnumerable<CountryDTO> GetAll()
        {
            var result = _context.CertificateCountry
                .Include(c => c.Country)
                .Include(c => c.Certificate)
                .Select(p => ModelToDTO(p.Country, p.Certificate))
                .ToList();

            return result;
        }

        [HttpPost]
        public Country Insert([FromBody]Country entity)
        {
            _context.Countries.Add(entity);
            return entity;
        }

        [HttpPut("[action]")]
        public IActionResult Add([FromBody]CountryDTO country)
        {
            Country newCountry = _context.Countries
                .FirstOrDefault(p => p.DescriptionEn == country.DescriptionEn);

            if (newCountry == null)
            {
                newCountry = new Country()
                {
                    Name = country.Name,
                    DescriptionEn = country.DescriptionEn,
                    DescriptionUa = country.DescriptionUa
                };
                _context.Countries.Add(newCountry);
            }


            if(!newCountry.CountryCertificates.Any(p=>p.CertificateId == country.CertificateId))
            newCountry.CountryCertificates.Add(new CertificateCountry
            {
                Country = newCountry,
                Certificate = _context.Certificates.GetById(country.CertificateId)
            }
            );

            _context.SaveChanges();

            return Ok();
        }

        [HttpPost("[action]")]
        public Country Save([FromBody]CountryDTO country)
        {
            Country currCountry = _context.Countries.GetById(country.CountryId);
            Certificate currCertificate = _context.Certificates.GetById(country.CertificateId);
            CertificateCountry certificateCountry = currCountry?.CountryCertificates
                .Where(p => p.Country == currCountry)
                .FirstOrDefault();

            if (currCountry != null && currCertificate != null)
            {
                currCountry.DescriptionEn = country?.DescriptionEn;
                currCountry.DescriptionUa = country?.DescriptionUa;
                if (certificateCountry == null)
                    currCountry.CountryCertificates.Add(certificateCountry);
                _context.SaveChanges();
            }

            return currCountry;
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            Country country = _context.Countries.FirstOrDefault(x => x.Id == id);
            try
            {
                if (country != null)
                {
                    _context.Countries.Remove(country);
                    _context.SaveChanges();
                }
                return Ok(country);
            }
            catch (SqlException ex)
            {
                return StatusCode(ex.ErrorCode);
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

                    List<Tuple<Country, List<Certificate>>> countries = ConvertHelper.CountryFromXLSToList(file.OpenReadStream(), _context.Certificates);
                    if (countries.Count() == 0)
                    {
                        return BadRequest();
                    }
                    foreach (var el in countries)
                    {
                        if (!_context.Countries.Any(p => p.Name == el.Item1.Name))
                            _context.Countries.Add(el.Item1);
                    }
                    //_context.SaveChanges();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return Json("Upload Failed: " + ex.Message);
            }

        }

        private CountryDTO ModelToDTO(Country country, Certificate certificate)
        {
            return new CountryDTO
            {
                Name = country.Name,
                DescriptionEn = country.DescriptionEn,
                DescriptionUa = country.DescriptionUa,
                CertificateId = certificate.Id,
                CountryId = country.Id
            };
        }
    }
}