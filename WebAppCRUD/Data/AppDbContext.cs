using Microsoft.EntityFrameworkCore;
using WebAppCRUD.Models;

namespace WebAppCRUD.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Contact> Contacts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Contact>()
                .HasDiscriminator<string>("ContactType")
                .HasValue<Contact>("Contact")
                .HasValue<BusinessContact>("BusinessContact");

            base.OnModelCreating(modelBuilder);
        }
    }
}