using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalticosAdmin.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdmin.AccesoDeDatos.Configuracion
{
    public class ProveedorConfiguracion : IEntityTypeConfiguration<Proveedor>
    {
        public void Configure(EntityTypeBuilder<Proveedor> builder)
        {
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.NombreEmpresa).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Contacto).HasMaxLength(50);
            builder.Property(x => x.Telefono).IsRequired().HasMaxLength(15);
            builder.Property(x => x.Correo).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Direccion).HasMaxLength(200);
            builder.Property(x => x.Descripcion).HasMaxLength(300);
            builder.Property(x => x.TipoProveedor).IsRequired().HasMaxLength(50);
        }
    }
}