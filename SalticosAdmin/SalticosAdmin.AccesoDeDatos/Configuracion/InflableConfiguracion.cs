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
    public class InflableConfiguracion : IEntityTypeConfiguration<Inflable>
    {
        public void Configure(EntityTypeBuilder<Inflable> builder)
        {
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.Nombre).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Descripcion).HasMaxLength(100);
            builder.Property(x => x.Dimensiones).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Estado).IsRequired();
            builder.Property(x => x.ImageUrl).IsRequired(false);
            builder.Property(x => x.Precio).IsRequired();
            builder.Property(x => x.PrecioHoraAdicional).IsRequired();
            builder.Property(x => x.CategoriaTamannoId).IsRequired();
            builder.Property(x => x.CategoriaEdadId).IsRequired();
            builder.Property(x => x.PadreId).IsRequired(false);

            //Relaciones
            builder.HasOne(x => x.CategoriaTamanno).WithMany()
                .HasForeignKey(x => x.CategoriaTamannoId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.CategoriasEdad).WithMany()
                .HasForeignKey(x => x.CategoriaEdadId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.Padre).WithMany()
                .HasForeignKey(x => x.PadreId)
                .OnDelete(DeleteBehavior.NoAction);

        }
    }
}