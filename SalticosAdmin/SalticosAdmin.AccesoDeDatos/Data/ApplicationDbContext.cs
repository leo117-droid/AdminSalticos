using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SalticosAdmin.Modelos;
using System.Reflection;

namespace SalticosAdmin.AccesoDeDatos.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
 
        }
        public DbSet<Herramienta> Herramientas { get; set; }
        public DbSet<Contacto> Contactos { get; set; }

        public DbSet<Cliente> Clientes { get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            
        }
    }
}
