using CreateInvoice.Entities;
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
                        if (worksheet.Cells[row, 1].Value.ToString().Trim() == "" || worksheet.Cells[row, 2].Value.ToString().Trim() == "")
                            continue;
                        DateTime dateEnd = DateTime.MinValue;
                        DateTime dateStart = DateTime.MinValue;

                        if(worksheet.Cells[row, 2].Value != null)
                            dateStart = DateTime.Parse(worksheet.Cells[row, 2].Value.ToString());

                        if(worksheet.Cells[row, 3].Value != null)
                            dateEnd = DateTime.Parse(worksheet.Cells[row, 3].Value.ToString());

                        //long dateStart = new DateTime( long.Parse(worksheet.Cells[row, 2].Value.ToString());
                        //long dateEnd = long.Parse(worksheet.Cells[row, 3].Value.ToString());

                        Certificate newCertificate = new Certificate()
                        {
                            Name = worksheet.Cells[row, 1].Value.ToString().Trim(),
                            StartDate = dateStart,
                            EndDate = dateEnd
                        };
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
    }
}
