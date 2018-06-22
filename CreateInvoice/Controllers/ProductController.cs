using System;
using System.Collections.Generic;
using System.IO;
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
    [Route("api/Product")]
    public class ProductController : Controller
    {
        private readonly ApplicationContext _context;

        public ProductController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet("[action]")]
        public async Task<IEnumerable<Product>> GetBySeq([FromQuery]string queryStr)
        {
            IEnumerable<Product> res = new List<Product>();
            res = await Task.FromResult(_context.Products
                .Include("CountryOfOrigin")
                .Where(p => p.CodeNo.Contains(queryStr)));

            if (res.Count() > 10)
                return res;

            return res;
        }

        [HttpGet("[action]")]
        public int GetProductsCount()
        {
            if(_context.Products.Any())
                return _context.Products.Count();
            return 0;
        }

        [HttpGet("[action]")]
        public IEnumerable<Product> GetAll([FromQuery]string skip, [FromQuery]string take)
        {
            var result = _context.Products
                .Include(p => p.CountryOfOrigin)
                .Include(p => p.Certificate)
                .Include(p => p.CountryOfOrigin).ToList();

            if (skip != null)
                result = result.Skip(int.Parse(skip)).ToList();
            if (take != null)
                result = result.Take(int.Parse(take)).ToList();

            return result;
        }

        [HttpPut("[action]")]
        public Product Add([FromBody]Product product)
        {
            Product newProduct = new Product()
            {
                CodeNo = product?.CodeNo,
                DescriptionEn = product?.DescriptionEn,
                DescriptionUa = product?.DescriptionUa,
                Certificate = _context.Certificates.GetById(product?.Certificate?.Id),
                CountryOfOrigin = _context.Countries.GetById(product?.CountryOfOrigin?.Id)
            };

            _context.Products.Add(newProduct);
            _context.SaveChanges();
            return newProduct;
        }

        [HttpPost("[action]")]
        public Product Save([FromBody]Product product)
        {
            Product currProduct = _context.Products.GetById(product.Id);
            if (currProduct != null)
            {
                currProduct.DescriptionEn = product.DescriptionEn;
                currProduct.DescriptionUa = product.DescriptionUa;
                currProduct.CodeNo = product.CodeNo;
                currProduct.Certificate = _context.Certificates.GetById(product.Certificate?.Id);
                currProduct.CountryOfOrigin = _context.Countries.GetById(product.CountryOfOrigin?.Id);
                _context.SaveChanges();
            }

            return currProduct;
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            Product product = _context.Products.FirstOrDefault(x => x.Id == id);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
            return Ok(product);
        }

        [HttpGet("[action]")]
        public IActionResult DownloadTemplate()
        {
            var locatedFile = System.IO.File.OpenRead(Path.Combine(Environment.CurrentDirectory, @"Templates\Product import template.xlsx"));
            var response = File(locatedFile, "application/octet-stream");

            return response;
        }

        [HttpPost("[action]")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> Upload()
        {
            int count = 1;
            try
            {
                var file = Request.Form.Files.FirstOrDefault();

                if (file?.Length > 0)
                {
                    List<ProductDTO> products = await Task.FromResult(ConvertHelper.GetProductsListFromUpload(file.OpenReadStream()));
                    if (products.Count() == 0)
                    {
                        return BadRequest();
                    }
                    foreach (var el in products)
                    {
                        Product newProduct = _context.Products
                            .Include(p => p.Certificate)
                            .Include(p => p.CountryOfOrigin)
                                .FirstOrDefault(p => p.DescriptionEn == el.DescriptionEn);

                        if (newProduct == null)
                        {
                            newProduct = new Product
                            {
                                CodeNo = el.CodeNo,
                                DescriptionEn = el.DescriptionEn,
                                DescriptionUa = el.DescriptionUa,
                                Certificate = _context.Certificates.FirstOrDefault(p => p.Name == el.CertificateName),
                                CountryOfOrigin = _context.Countries.FirstOrDefault(p => p.DescriptionEn == el.DescriptionEn)
                            };

                            if (newProduct.Certificate == null)
                            {
                                if (el.CertificateName != "" && el.CertificateStartDate != null)
                                {
                                    Certificate newCertificate = new Certificate
                                    {
                                        Name = el.CertificateName,
                                        StartDate = el.CertificateStartDate ?? DateTime.MinValue,
                                        EndDate = el.CertificateEndDate
                                    };

                                    _context.Certificates.Add(newCertificate);
                                    _context.SaveChanges();
                                    newProduct.Certificate = newCertificate;
                                }
                            }

                            if (newProduct.Certificate != null && newProduct.CountryOfOrigin == null)
                            {
                                if (el.CountryDescriptionEn != "" && el.CountryDescriptionEn != null)
                                {
                                    Country newCountry = new Country
                                    {
                                        DescriptionEn = el.DescriptionEn,
                                        Name = el.CountryName
                                    };

                                    newCountry.CountryCertificates.Add(
                                        new CertificateCountry
                                        {
                                            Country = newCountry,
                                            Certificate = newProduct.Certificate
                                        });
                                    _context.Countries.Add(newCountry);
                                    _context.SaveChanges();
                                    newProduct.CountryOfOrigin = newCountry;
                                }
                            }

                            if (newProduct.Certificate != null)
                                _context.Products.Add(newProduct);
                        }
                        else
                        {
                            if (newProduct.Certificate == null)
                            {
                                if (el.CertificateName != "" && el.CertificateStartDate != null)
                                {
                                    Certificate newCertificate = new Certificate
                                    {
                                        Name = el.CertificateName,
                                        StartDate = el.CertificateStartDate ?? DateTime.MinValue,
                                        EndDate = el.CertificateEndDate
                                    };

                                    _context.Certificates.Add(newCertificate);
                                    _context.SaveChanges();
                                    newProduct.Certificate = newCertificate; 
                                }
                            }

                            if (newProduct.Certificate != null && newProduct.CountryOfOrigin == null)
                            {
                                if (el.CountryDescriptionEn != "" && el.CountryDescriptionEn != null)
                                {
                                    Country newCountry = new Country
                                    {
                                        DescriptionEn = el.DescriptionEn,
                                        Name = el.CountryName
                                    };

                                    newCountry.CountryCertificates.Add(
                                        new CertificateCountry
                                        {
                                            Country = newCountry,
                                            Certificate = newProduct.Certificate
                                        });
                                    _context.Countries.Add(newCountry);
                                    _context.SaveChanges();
                                    newProduct.CountryOfOrigin = newCountry;
                                }
                            }


                        }

                        count++;
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

        [HttpPost("[action]")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadFromInvoice()
        {
            try
            {
                var file = Request.Form.Files.FirstOrDefault();

                if (file?.Length > 0)
                {
                    List<ProductDTO> products = await Task.FromResult(ConvertHelper.GetProductsListFromInvoiceUpload(file.OpenReadStream()));
                    if (products.Count() == 0)
                    {
                        return BadRequest();
                    }
                    foreach (var el in products)
                    {
                        Product newProduct = _context.Products
                            .Include(p => p.Certificate)
                            .Include(p => p.CountryOfOrigin)
                                .FirstOrDefault(p => p.DescriptionEn == el.DescriptionEn);

                        if (newProduct == null)
                        {
                            newProduct = new Product
                            {
                                CodeNo = el.CodeNo,
                                DescriptionEn = el.DescriptionEn,
                                DescriptionUa = el.DescriptionUa,
                                Certificate = _context.Certificates.FirstOrDefault(p => p.Name == el.CertificateName),
                                CountryOfOrigin = _context.Countries.FirstOrDefault(p => p.DescriptionEn == el.DescriptionEn)
                            };

                            if (newProduct.Certificate == null)
                            {
                                if (el.CertificateName != "")
                                {
                                    Certificate newCertificate = new Certificate
                                    {
                                        Name = el.CertificateName,
                                        StartDate = el.CertificateStartDate ?? DateTime.MinValue,
                                        EndDate = el.CertificateEndDate
                                    };
                                    _context.Certificates.Add(newCertificate);
                                    newProduct.Certificate = newCertificate;
                                }
                            }

                            if (newProduct.Certificate != null && newProduct.CountryOfOrigin == null)
                            {
                                if (el.CountryDescriptionEn != "")
                                {
                                    Country newCountry = new Country
                                    {
                                        DescriptionEn = el.DescriptionEn,
                                        Name = el.CountryName
                                    };

                                    newCountry.CountryCertificates.Add(
                                        new CertificateCountry
                                        {
                                            Country = newCountry,
                                            Certificate = newProduct.Certificate
                                        });
                                    _context.Countries.Add(newCountry);
                                    newProduct.CountryOfOrigin = newCountry;
                                }
                            }

                            if (newProduct.Certificate != null && newProduct.CountryOfOrigin != null)
                                _context.Products.Add(newProduct);
                        }
                        else
                        {
                            if (newProduct.Certificate == null)
                            {
                                if (el.CertificateName != "")
                                {
                                    Certificate newCertificate = new Certificate
                                    {
                                        Name = el.CertificateName,
                                        StartDate = el.CertificateStartDate ?? DateTime.MinValue,
                                        EndDate = el.CertificateEndDate
                                    };
                                    _context.Certificates.Add(newCertificate);
                                    newProduct.Certificate = newCertificate;
                                }
                            }

                            if (newProduct.Certificate != null && newProduct.CountryOfOrigin == null)
                            {
                                if (el.CountryDescriptionEn != "")
                                {
                                    Country newCountry = new Country
                                    {
                                        DescriptionEn = el.DescriptionEn,
                                        Name = el.CountryName
                                    };

                                    newCountry.CountryCertificates.Add(
                                        new CertificateCountry
                                        {
                                            Country = newCountry,
                                            Certificate = newProduct.Certificate
                                        });
                                    _context.Countries.Add(newCountry);
                                    newProduct.CountryOfOrigin = newCountry;
                                }
                            }
                        }
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
    }
}