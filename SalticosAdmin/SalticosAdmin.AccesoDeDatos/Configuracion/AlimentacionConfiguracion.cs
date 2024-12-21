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
    public class AlimentacionConfiguracion : IEntityTypeConfiguration<Alimentacion>
    {
        public void Configure(EntityTypeBuilder<Alimentacion> builder)
        {
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.Nombre).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Descripcion).HasMaxLength(100);
            builder.Property(x => x.ImageUrl).IsRequired(false);
            builder.Property(x => x.Precio).IsRequired();
        }
    }
}