using CreateInvoice.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CreateInvoice.Helpers
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
           : base(options)
        { }

        public DbSet<Organization> Organizations { get; set; }
        public DbSet<TermsOfDelivery> TermsOfDelivery { get; set; }
        public DbSet<DeliveryType> DeliveryTypes { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<TermOfPayment> TermsOfPayment { get; set; }

    }
}
