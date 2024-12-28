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
    public class AlimentacioningredienteConfiguracion : IEntityTypeConfiguration<AlimentacionIngrediente>
    {
        public void Configure(EntityTypeBuilder<AlimentacionIngrediente> builder)
        {
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.IdIngrediente).IsRequired();
            builder.Property(x => x.IdAlimentacion).IsRequired();


            builder.HasOne(x => x.Ingrediente).WithMany()
                .HasForeignKey(x => x.IdIngrediente).
                OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.Alimentacion).WithMany()
                .HasForeignKey(x => x.IdAlimentacion).
                OnDelete(DeleteBehavior.NoAction);

        }
    }
}