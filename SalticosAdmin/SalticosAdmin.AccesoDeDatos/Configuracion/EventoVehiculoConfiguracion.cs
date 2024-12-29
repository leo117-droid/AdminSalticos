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
    public class EventoVehiculoConfiguracion : IEntityTypeConfiguration<EventoVehiculo>
    {
        public void Configure(EntityTypeBuilder<EventoVehiculo> builder)
        {
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.IdVehiculo).IsRequired();
            builder.Property(x => x.IdEvento).IsRequired();

            builder.HasOne(x => x.Vehiculo).WithMany()
                .HasForeignKey(x => x.IdVehiculo).
                OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.Evento).WithMany()
                .HasForeignKey(x => x.IdEvento).
                OnDelete(DeleteBehavior.NoAction);

        }
    }
}