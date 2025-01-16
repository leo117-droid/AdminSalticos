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
    public class EventoConfiguracion : IEntityTypeConfiguration<Evento>
    {
        public void Configure(EntityTypeBuilder<Evento> builder)
        {
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.Fecha).IsRequired();
            builder.Property(x => x.HoraInicio).IsRequired();
            builder.Property(x => x.HoraFinal).IsRequired();
            builder.Property(x => x.Direccion).IsRequired().HasMaxLength(120);
            builder.Property(x => x.Provincia).IsRequired().HasMaxLength(20);
            builder.Property(x => x.ClienteId).IsRequired();
            builder.Property(x => x.EstadoRecordatorio).IsRequired();

            //Relaciones
            builder.HasOne(x => x.Cliente).WithMany()
                .HasForeignKey(x => x.ClienteId)
                .OnDelete(DeleteBehavior.NoAction);

        }
    }
}