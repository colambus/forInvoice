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
    [Route("api/Country")]
    public class CountryController : Controller
    {
        private readonly ApplicationContext _context;

        public CountryController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet("[action]")]
        public IEnumerable<Country> GetAll()
        {
            return _context.Countries.Include(p => p.Certificate);
        }

        [HttpPost]
        public Country Insert([FromBody]Country entity)
        {
            _context.Countries.Add(entity);
            return entity;
        }

        [HttpPut("[action]")]
        public Country Add([FromBody]Country country)
        {
            Country newCountry = new Country()
            {
                DescriptionEn = country.DescriptionEn,
                DescriptionUa = country.DescriptionUa,
                Certificate = _context.Certificates.GetById(country.Certificate?.Id)
            };

            _context.Countries.Add(newCountry);
            _context.SaveChanges();
            return newCountry;
        }

        [HttpPost("[action]")]
        public Country Save([FromBody]Country country)
        {
            Country currCountry = _context.Countries.GetById(country.Id);
            if (currCountry != null)
            {
                currCountry.DescriptionEn = country.DescriptionEn;
                currCountry.DescriptionUa = country.DescriptionUa;
                currCountry.Certificate = _context.Certificates.GetById(country.Certificate?.Id);
                _context.SaveChanges();
            }

            return currCountry;
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            Country country = _context.Countries.FirstOrDefault(x => x.Id == id);
            if (country != null)
            {
                _context.Countries.Remove(country);
                _context.SaveChanges();
            }
            return Ok(country);
        }
    }
}