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
    public class CapacitacionConfiguracion : IEntityTypeConfiguration<Capacitacion>
    {
        public void Configure(EntityTypeBuilder<Capacitacion> builder)
        {
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.Fecha).IsRequired().HasColumnType("date");
            builder.Property(x => x.Tema).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Duracion).IsRequired().HasMaxLength(50);

        }
    }
}