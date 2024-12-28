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
    public class EventoAlimentacionConfiguracion : IEntityTypeConfiguration<EventoAlimentacion>
    {
        public void Configure(EntityTypeBuilder<EventoAlimentacion> builder)
        {
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.IdAlimentacion).IsRequired();
            builder.Property(x => x.IdEvento).IsRequired();
            builder.Property(x => x.Cantidad).IsRequired().HasDefaultValue(1); 

            // Restricción para asegurar que la cantidad sea mayor a 0
            builder.HasCheckConstraint("CK_EventoAlimentacion_Cantidad", "Cantidad > 0");

            builder.HasOne(x => x.Alimentacion).WithMany()
                .HasForeignKey(x => x.IdAlimentacion).
                OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.Evento).WithMany()
                .HasForeignKey(x => x.IdEvento).
                OnDelete(DeleteBehavior.NoAction);

        }
    }
}