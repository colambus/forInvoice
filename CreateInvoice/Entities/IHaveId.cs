using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CreateInvoice.Entities
{
    public interface IHaveId
    {
        int Id { get; set; }
    }
}
