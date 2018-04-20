using CreateInvoice.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace CreateInvoice.ViewModel
{
    public class NamedModel: INamed
    {
        public NamedModel(int _id, string _name)
        {
            Id = _id;
            Name = _name;
        }

        public int Id { get; set; }
        public string Name { get; set; }
    }
}
