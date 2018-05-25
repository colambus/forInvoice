using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CreateInvoice.Entities
{
    public class ContactDetails: INamed
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual Organization Organization { get; set; }
        public virtual ContactType ContactType { get; set; }
    }
}
