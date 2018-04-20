using CreateInvoice.Entities;
using CreateInvoice.ViewModel;
using System;
using System.Collections.Generic;
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
    }
}
