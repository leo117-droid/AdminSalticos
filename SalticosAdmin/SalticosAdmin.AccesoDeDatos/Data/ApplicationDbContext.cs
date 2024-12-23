﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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
        public DbSet<TarifasTransporte> TarifasTransportes { get; set; }

        public DbSet<Vehiculo> Vehiculos { get; set; }

        public DbSet<CategoriasEdad> CategoriasEdades { get; set; }

        public DbSet<CategoriaTamanno> CategoriaTammano { get; set; }

        public DbSet<RolPersonal> RolPersonal { get; set; }

        public DbSet<Ingrediente> Ingredientes { get; set; }

        public DbSet<Personal> Personal { get; set; }

        public DbSet<Capacitacion> Capacitaciones { get; set; }

        public DbSet<Inflable> Inflables { get; set; }
        public DbSet<ServicioAdicional> ServiciosAdicionales { get; set; }

        public DbSet<Mobiliario> Mobilarios { get; set; }
        public DbSet<Alimentacion> Alimentacion { get; set; }
        public DbSet<Bitacora> Bitacora { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            
        }
    }
}
