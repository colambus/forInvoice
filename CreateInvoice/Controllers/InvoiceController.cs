using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CreateInvoice.Entities;
using CreateInvoice.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace CreateInvoice.Controllers
{
    [Produces("application/json")]
    [Route("api/Invoice")]
    public class InvoiceController : Controller
    {
        private readonly ApplicationContext _context;

        public InvoiceController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet("[action]")]
        public IEnumerable<Invoice> GetAll()
        {
            return _context.Invoices
                .ToList();
        }

        [HttpGet("[action]")]
        public Invoice CreateNewInvoice()
        {
            Invoice invoice = new Invoice
            {
                Date = DateTime.Now,
                InvoiceNo = InvoiceHelper.CreateNewNo(DateTime.Now, GetInvoiceCount(DateTime.Now))
            };
            _context.Invoices.Add(invoice);
            _context.SaveChanges();

            return invoice;
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            Invoice invoice = _context.Invoices.FirstOrDefault(x => x.Id == id);
            if (invoice != null)
            {
                _context.Invoices.Remove(invoice);
                _context.SaveChanges();
            }
            return Ok(invoice);
        }

        [HttpPost("[action]")]
        public Invoice GetById([FromBody]Invoice invoice)
        {
            Invoice result = _context.Invoices
                .Include(p => p.Buyer)
                .Include(p => p.Contract)
                .Include(p => p.DeliveryType)
                .Include(p => p.Seller)
                .Include(p => p.TermOfPayment)
                .Include(p => p.TermsOfDelivery)
                .FirstOrDefault(x => x.Id == invoice.Id);

            return result;
        }

        [HttpPut("[action]")]
        public Invoice Save([FromBody]Invoice invoice)
        {
            Invoice currInvoice = _context.Invoices.Where(i => i.Id == invoice.Id).FirstOrDefault();
            SetNewValues(ref currInvoice, invoice);

            currInvoice.InvoiceNo = InvoiceHelper.CreateNewNo(invoice.Date, GetInvoiceCount(invoice.Date));
            _context.SaveChanges();
            return invoice;
        }

        [HttpGet("[action]")]
        public IActionResult Download([FromQuery]string id)
        {
            var stream = new MemoryStream();
            Invoice invoice = _context.Invoices
                .Where(p => p.Id == int.Parse(id))
                .Include(p => p.Buyer).Include(p => p.Seller)
                .Include(p => p.TermOfPayment).Include(p => p.TermsOfDelivery)
                .Include(p => p.DeliveryType).Include(p => p.Contract).FirstOrDefault();

            if (invoice == null)
                return BadRequest();

            var seller = _context.Organizations.GetById(invoice.Seller.Id);
            var buyer = _context.Organizations.GetById(invoice.Buyer.Id);
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Invoice");
                worksheet.Cells[1, 1, 1, 4].Merge = true;
                worksheet.Cells[1, 1, 1, 4].Value = seller.Name;
                worksheet.Cells[1, 1, 1, 4].Style.Font.Bold = true;
                worksheet.Cells[1, 1, 1, 4].Style.Font.Size = 14;
                worksheet.Cells[1, 5, 1, 8].Merge = true;
                worksheet.Cells[1, 5, 1, 8].Value = "Invoice";
                worksheet.Cells[1, 5, 1, 8].Style.Font.Size = 14;
                worksheet.Cells[1, 5, 1, 8].Style.Font.Bold = true;
                worksheet.Cells[2, 5, 2, 6].Merge = true;
                worksheet.Cells[2, 5, 2, 6].Value = "Invoice No:";
                worksheet.Cells[2, 5, 2, 6].Style.Font.Size = 10;
                worksheet.Cells[2, 5, 2, 6].Style.Font.Bold = true;
                worksheet.Cells[3, 5, 3, 6].Merge = true;
                worksheet.Cells[3, 5, 3, 6].Value = "Invoice date:";
                worksheet.Cells[3, 5, 3, 6].Style.Font.Size = 10;
                worksheet.Cells[3, 5, 3, 6].Style.Font.Bold = true;
                worksheet.Cells[4, 5, 4, 6].Merge = true;
                worksheet.Cells[4, 5, 4, 6].Value = "Invoice date:";
                worksheet.Cells[4, 5, 4, 6].Style.Font.Size = 10;
                worksheet.Cells[4, 5, 4, 6].Style.Font.Bold = true;
                worksheet.Cells[2, 7, 2, 8].Merge = true;
                worksheet.Cells[2, 7, 2, 8].Value = invoice.InvoiceNo;
                worksheet.Cells[2, 7, 2, 8].Style.Font.Size = 10;
                worksheet.Cells[4, 7, 4, 8].Merge = true;
                worksheet.Cells[4, 7, 4, 8].Value = invoice.Contract?.Name;
                worksheet.Cells[4, 7, 4, 8].Style.Font.Size = 10;
                worksheet.Cells[3, 7, 3, 8].Merge = true;
                worksheet.Cells[3, 7, 3, 8].Value = invoice.Date;
                worksheet.Cells[3, 7, 3, 8].Style.Font.Size = 10;
                worksheet.Cells[6, 1, 6, 3].Merge = true;
                worksheet.Cells[6, 1, 6, 3].Value = "Invoice to:";
                worksheet.Cells[6, 1, 6, 3].Style.Font.Bold = true;
                worksheet.Cells[6, 1, 6, 3].Style.Font.Size = 10;
                worksheet.Cells[7, 1, 7, 3].Merge = true;
                worksheet.Cells[7, 1, 7, 3].Value = invoice.Buyer.Name;
                worksheet.Cells[7, 1, 7, 3].Style.Font.Size = 10;
                worksheet.Cells[8, 1, 10, 3].Merge = true;
                worksheet.Cells[8, 1, 10, 3].Value = invoice.Buyer.Description;
                worksheet.Cells[8, 1, 10, 3].Style.Font.Size = 10;
                worksheet.Cells[7, 4].Merge = true;
                worksheet.Cells[7, 4].Value = "Ship to:";
                worksheet.Cells[7, 4].Style.Font.Bold = true;
                worksheet.Cells[7, 4].Style.Font.Size = 10;
                worksheet.Cells[7, 4].Merge = true;
                worksheet.Cells[7, 4].Value = invoice.Buyer.Name;
                worksheet.Cells[7, 4].Style.Font.Size = 10;
                worksheet.Cells[8, 4, 10, 4].Merge = true;
                worksheet.Cells[8, 4, 10, 4].Value = invoice.Buyer.Description;
                worksheet.Cells[8, 4, 10, 4].Style.Font.Size = 10;
                worksheet.Cells[6, 5, 6, 8].Value = "payment identification no.:";
                worksheet.Cells[6, 5, 6, 8].Style.Font.Size = 10;
                worksheet.Cells[6, 5, 6, 8].Merge = true;
                worksheet.Cells[2, 5, 5, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[6, 1, 10, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[6, 4, 10, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[6, 5, 10, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                worksheet.Cells[12, 1].Value = "Pos. No.";
                worksheet.Cells[12, 2].Value = "Country of origin";
                worksheet.Cells[12, 3].Value = "Code No.";
                worksheet.Cells[12, 4].Value = "Description";
                worksheet.Cells[12, 5].Value = "Unit/Pack";
                worksheet.Cells[12, 6].Value = "Unit Qty";
                worksheet.Cells[12, 7].Value = "Unit Price";
                worksheet.Cells[12, 8].Value = "Amount";
                worksheet.Cells[12, 8].Style.Font.Bold = true;
                for (int i = 1; i < 8; i++)
                {
                    worksheet.Cells[12, i].Style.Font.Bold = true;
                    worksheet.Cells[12, i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[12, i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[12, i].Style.WrapText = true;
                }
                worksheet.Cells[12, 1, 12, 8].Style.Border.Top.Style = ExcelBorderStyle.Double;
                worksheet.Cells[12, 1, 12, 8].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
                stream = new MemoryStream(package.GetAsByteArray());
            }


            var response = File(stream, "application/octet-stream");

            return response;
        }

        private int GetInvoiceCount(DateTime date)
        {
            var numberOfInvoices = _context.Invoices
                .Where(d => d.Date >= DateTime.Now.Date.Date && d.Date >= DateTime.Now.Date.AddDays(1).AddTicks(-1))
                .Count() + 1;

            return numberOfInvoices;
        }

        private Invoice SetNewValues(ref Invoice currInvoice, Invoice invoice)
        {
            currInvoice.Seller = _context.Organizations.GetById(invoice.Seller?.Id);
            currInvoice.Buyer = _context.Organizations.GetById(invoice.Buyer?.Id);
            currInvoice.Contract = _context.Contracts.GetById(invoice.Contract?.Id);
            currInvoice.Date = invoice.Date;
            currInvoice.DeliveryType = _context.DeliveryTypes.GetById(invoice.DeliveryType?.Id);
            currInvoice.TermOfPayment = _context.TermsOfPayment.GetById(invoice.TermOfPayment?.Id);
            currInvoice.TermsOfDelivery = _context.TermsOfDelivery.GetById(invoice.TermOfPayment?.Id);

            return currInvoice;
        }
    }
}