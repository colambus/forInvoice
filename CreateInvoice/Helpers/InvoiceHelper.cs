using CreateInvoice.Entities;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CreateInvoice.Helpers
{
    public static class InvoiceHelper
    {
        public static string CreateNewNo(DateTime date, int numberOfInvoices)
        {
            string newNumber = "HB";
            newNumber = newNumber + date.Year.ToString() + date.Month + date.Day + "-" + numberOfInvoices;
            return newNumber;
        }

        public static T GetById<T>(this IEnumerable<T> source, int? id) where T: IHaveId
        {
            return source.FirstOrDefault(p => p.Id == id);
        }       
    }
}
