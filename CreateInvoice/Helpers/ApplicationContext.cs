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
        public DbSet<Product> Products { get; set; }
        public DbSet<InvoiceProducts> InvoiceProducts { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<ContactType> ContactTypes { get; set; }
        public DbSet<ContactDetails> ContactDetails { get; set; }
        public DbSet<Certificate> Certificates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .HasIndex(p => new { p.CodeNo});
        }
    }
}
