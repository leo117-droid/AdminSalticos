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
    public class EventoInflableConfiguracion : IEntityTypeConfiguration<EventoInflable>
    {
        public void Configure(EntityTypeBuilder<EventoInflable> builder)
        {
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.IdInflable).IsRequired();
            builder.Property(x => x.IdEvento).IsRequired();

            builder.HasOne(x => x.Inflable).WithMany()
                .HasForeignKey(x => x.IdInflable).
                OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.Evento).WithMany()
                .HasForeignKey(x => x.IdEvento).
                OnDelete(DeleteBehavior.NoAction);

        }
    }
}