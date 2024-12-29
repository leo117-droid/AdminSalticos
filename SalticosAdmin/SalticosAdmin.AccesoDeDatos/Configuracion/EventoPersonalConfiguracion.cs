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
    public class EventoPersonalConfiguracion : IEntityTypeConfiguration<EventoPersonal>
    {
        public void Configure(EntityTypeBuilder<EventoPersonal> builder)
        {
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.IdPersonal).IsRequired();
            builder.Property(x => x.IdEvento).IsRequired();

            builder.HasOne(x => x.Personal).WithMany()
                .HasForeignKey(x => x.IdPersonal).
                OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.Evento).WithMany()
                .HasForeignKey(x => x.IdEvento).
                OnDelete(DeleteBehavior.NoAction);

        }
    }
}