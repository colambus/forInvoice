﻿using CreateInvoice.Entities;
using CreateInvoice.ViewModel;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CreateInvoice.Helpers
{
    public static class ConvertHelper
    {
        public static List<NamedModel> ToNamed(this List<INamed> namedList)
        {
            List<NamedModel> result = new List<NamedModel>();
            foreach (var el in namedList)
                result.Add(new NamedModel(el.Id, el.Name));
            return result;
        }

        public static List<Certificate> CertificatesFromXLSToList(Stream file)
        {
            List<Certificate> certificates = new List<Certificate>();

            using (ExcelPackage package = new ExcelPackage(file))
            {
                try
                {
                    // add a new worksheet to the empty workbook
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.First();
                    var start = worksheet.Dimension.Start;
                    var end = worksheet.Dimension.End;
                    for (int row = start.Row + 1; row <= end.Row; row++)
                    {
                        if (worksheet.Cells[row, 1].Value == null || worksheet.Cells[row, 2].Value == null)
                            continue;
                        DateTime dateEnd = DateTime.MinValue;
                        DateTime dateStart = DateTime.MinValue;

                        if (worksheet.Cells[row, 2].Value != null)
                            dateStart = DateTime.Parse(worksheet.Cells[row, 2].Value.ToString());

                        if (worksheet.Cells[row, 3].Value != null)
                            dateEnd = DateTime.Parse(worksheet.Cells[row, 3].Value.ToString());

                        Certificate newCertificate = new Certificate()
                        {
                            Name = worksheet.Cells[row, 1].Value.ToString().Trim(),
                            StartDate = dateStart,
                            EndDate = dateEnd
                        };

                        if (!certificates.Any(p => p.Name == newCertificate.Name))
                            certificates.Add(newCertificate);
                    }
                }
                catch
                {
                    certificates = new List<Certificate>();
                    return certificates;
                };
            }

            return certificates;
        }

        public static List<Tuple<Country, List<Certificate>>> CountryFromXLSToList(Stream file, IEnumerable<Certificate> certificates)
        {
            List<Country> countries = new List<Country>();
            List<Tuple<Country, List<Certificate>>> result = new List<Tuple<Country, List<Certificate>>>();
            
            List<CountryDTO> countrieDTOs = new List<CountryDTO>();
            try
            {
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.First();
                    var start = worksheet.Dimension.Start;
                    var end = worksheet.Dimension.End;
                    List<CertificateCountry> certCountry = GetInvoiceCountryCert(worksheet, certificates);

                    var countriesStartRange = from cell in worksheet.Cells
                                              where cell.Value?.ToString().Trim() == "Country of origin"
                                              select cell.Start;

                    ExcelCellAddress countriesStart = null;
                    ExcelCellAddress countriesTableStart = null;
                    foreach (var cell in countriesStartRange)
                        if (worksheet.Cells[cell.Row, cell.Column + 2].Value?.ToString().Trim() == "Manufacturing" ||
                            worksheet.Cells[cell.Row, cell.Column + 2].Value?.ToString().Trim() == "Manufacturer")
                        {
                            countriesStart = cell;
                            break;
                        }
                        else if (worksheet.Cells[cell.Row, cell.Column + 2].Value?.ToString().Trim() == "Description")
                        {
                            countriesTableStart = cell;
                        }

                    if (countriesStart != null)
                    {
                        while (worksheet.Cells[countriesStart.Row + 1, countriesStart.Column + 1].Value != null &&
                            worksheet.Cells[countriesStart.Row + 1, countriesStart.Column + 1].Value?.ToString().Trim() != "" &&
                            worksheet.Cells[countriesStart.Row + 1, 1].Value?.ToString().Trim() != "Information")
                        {
                            Country newCountry = new Country
                            {
                                Name = worksheet.Cells[countriesStart.Row + 1, countriesStart.Column + 1].Value?.ToString().Trim(),
                                DescriptionEn = worksheet.Cells[countriesStart.Row + 1, countriesStart.Column + 2].Value?.ToString().Trim()
                            };

                            if (worksheet.Cells[countriesStart.Row + 1, countriesStart.Column].Value != null)
                            {
                                int countryId = int.Parse(worksheet.Cells[countriesStart.Row + 1, countriesStart.Column].Value.ToString());
                                result.Add(new Tuple<Country, List<Certificate>>(newCountry, new List<Certificate>(certCountry.Where(p => p.CountryId == countryId).Select(p => p.Certificate).Distinct())));
                            }

                            countriesStart = worksheet.Cells[countriesStart.Row + 1, countriesStart.Column].Start;
                        }
                    }                  
                }
            }
            catch (Exception ex)
            {
                result = new List<Tuple<Country, List<Certificate>>>();
                return result;
            };

            return result;
        }

        private static List<CertificateCountry> GetInvoiceCountryCert(ExcelWorksheet worksheet, IEnumerable<Certificate> certificates)
        {
            List<CertificateCountry> certCountry = new List<CertificateCountry>();

            try
            {
                var certColumn = (from column in worksheet.Cells
                                  where (column.Value != null && column.Value.ToString().Contains("Amount"))
                                  select column)
                                     .FirstOrDefault().Start.Column + 1;

                var posRange = (from column in worksheet.Cells
                                where (column.Value != null && column.Value.ToString().Contains("Pos. No."))
                                select column)
                                     .FirstOrDefault().Start;

                int startRow = 0, endRow = 0;

                if (posRange != null)
                {
                    endRow = posRange.Row;
                    startRow = posRange.Row;
                    while (worksheet.Cells[endRow + 1, 1].Value != null &&
                    worksheet.Cells[endRow + 1, 1].Value?.ToString().Trim() != "")
                    {
                        endRow++;
                    }
                }

                for (int i = startRow + 1; i <= endRow; i++)
                {
                    var cert = certificates.FirstOrDefault(p => p.Name == worksheet.Cells[i, certColumn].Value?.ToString().Trim());

                    if (cert != null)
                        certCountry.Add(new CertificateCountry
                        {
                            CountryId = int.Parse(worksheet.Cells[i, 2].Value.ToString()),
                            CertificateId = cert.Id,
                            Certificate = cert
                        });
                }
            }
            catch
            {
                certCountry.Clear();
                return certCountry;
            }

            return certCountry;
        }

        public static List<ProductDTO> GetProductsListFromUpload(Stream file)
        {
            List<ProductDTO> products = new List<ProductDTO>();
            try
            {
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.First();
                    var start = worksheet.Dimension.Start;
                    var end = worksheet.Dimension.End;

                    for(int row = 2; row<= end.Row; row++)
                    {
                        ProductDTO newProduct = new ProductDTO
                        {
                            CodeNo = worksheet.Cells[row, 1].Value?.ToString().Trim(),
                            DescriptionEn = worksheet.Cells[row, 2].Value?.ToString().Trim(),
                            DescriptionUa = worksheet.Cells[row, 3].Value?.ToString().Trim(),
                            CertificateName = worksheet.Cells[row, 4].Value?.ToString().Trim(),
                            CertificateStartDate = DateTime.Parse(worksheet.Cells[row, 5].Value?.ToString().Trim()),
                            CertificateEndDate = worksheet.Cells[row, 6].Value == null ? (DateTime?)null : DateTime.Parse(worksheet.Cells[row, 6].Value?.ToString().Trim()),
                            CountryDescriptionEn = worksheet.Cells[row, 7].Value?.ToString().Trim(),
                            CountryName = worksheet.Cells[row, 8].Value?.ToString().Trim()
                        };
                        products.Add(newProduct);
                    }

                }
            }
            catch
            {
                products.Clear();
            }            

                return products;
        }

        public static List<ProductDTO> GetProductsListFromInvoiceUpload(Stream file)
        {
            List<ProductDTO> products = new List<ProductDTO>();
            try
            {
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.First();
                    var start = worksheet.Dimension.Start;
                    var end = worksheet.Dimension.End;

                    ExcelCellAddress productsStartCell = (from cell in worksheet.Cells
                                              where cell.Value?.ToString().Trim() == "Pos. No."
                                              select cell.Start).FirstOrDefault();

                    ExcelCellAddress countriesStartCell = (from cell in worksheet.Cells
                                              where cell.Value?.ToString().Trim() == "Country of origin"
                                              select cell.Start).LastOrDefault();

                    ExcelCellAddress amountCell = (from column in worksheet.Cells
                                                   where (column.Value != null && column.Value.ToString().Contains("Amount"))
                                                   select column)
                                                    .FirstOrDefault().Start;

                    ExcelCellAddress countryIdCell = (from column in worksheet.Cells
                                                   where (column.Value != null && column.Value.ToString().Contains("Country of origin"))
                                                   select column)
                                                    .FirstOrDefault().Start;

                    ExcelCellAddress productNameCell = (from column in worksheet.Cells
                                                        where (column.Value != null && column.Value.ToString().Contains("Description"))
                                                        select column)
                                                    .FirstOrDefault().Start;

                    ExcelCellAddress productCodeCell = (from column in worksheet.Cells
                                                        where (column.Value != null && column.Value.ToString().Contains("Code No."))
                                                        select column)
                                                    .FirstOrDefault().Start;

                    if (productsStartCell != null && amountCell != null && countryIdCell != null && productNameCell != null && productCodeCell != null)
                    {
                        productsStartCell = worksheet.Cells[productsStartCell.Row+1, productsStartCell.Column].Start;
                        while (worksheet.Cells[productsStartCell.Row, productsStartCell.Column].Value != null &&
                              worksheet.Cells[productsStartCell.Row, productsStartCell.Column].Value?.ToString().Trim() != "")
                        {
                            ProductDTO newproduct = new ProductDTO
                            {
                                CodeNo = worksheet.Cells[productsStartCell.Row, productCodeCell.Column].Value?.ToString().Trim(),
                                DescriptionEn = worksheet.Cells[productsStartCell.Row, productNameCell.Column].Value?.ToString().Trim(),
                                CertificateName = worksheet.Cells[productsStartCell.Row, amountCell.Column + 1].Value?.ToString().Trim()                              
                            };                           

                            ExcelCellAddress currCountryCell = worksheet.Cells[countriesStartCell.Row + 1, countriesStartCell.Column].Start;

                            if (countriesStartCell != null)
                            {
                                while (worksheet.Cells[currCountryCell.Row, currCountryCell.Column].Value != null &&
                                worksheet.Cells[currCountryCell.Row, currCountryCell.Column].Value?.ToString().Trim() != "")
                                {
                                    if (worksheet.Cells[currCountryCell.Row, currCountryCell.Column].Value?.ToString() ==
                                        worksheet.Cells[productsStartCell.Row, countryIdCell.Column].Value?.ToString())
                                    {
                                        newproduct.CountryDescriptionEn = worksheet.Cells[currCountryCell.Row, currCountryCell.Column + 2].Value?.ToString().Trim();
                                        newproduct.CountryName = worksheet.Cells[currCountryCell.Row, currCountryCell.Column + 1].Value?.ToString().Trim();
                                        break;
                                    }
                                    currCountryCell = worksheet.Cells[currCountryCell.Row + 1, currCountryCell.Column].Start;
                                }
                            }

                            productsStartCell = worksheet.Cells[productsStartCell.Row + 1, productsStartCell.Column].Start;

                            products.Add(newproduct);
                        }
                    }
                }
            }
            catch
            {
                products.Clear();
            }

            return products;
        }
    }
}
