using ArreglaMiCiudad.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ArreglaMiCiudad.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Administrator> Administrators { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<ReportImage> ReportImages { get; set; }
        public DbSet<Category> Categories { get; set; }
        }
}
