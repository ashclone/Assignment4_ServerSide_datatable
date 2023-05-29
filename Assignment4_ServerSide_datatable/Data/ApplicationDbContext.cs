using Assignment4_ServerSide_datatable.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Assignment4_ServerSide_datatable.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Employee> EmployeeData { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Employee>()
                .Property(p => p.Gender)
                .HasConversion(
                    v => v.ToString(),
                    v => (Gender)Enum.Parse(typeof(Gender), v));
        }
    }
}
