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
    public class SeguroConfiguracion : IEntityTypeConfiguration<Seguro>
    {
        public void Configure(EntityTypeBuilder<Seguro> builder)
        {
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.TipoSeguro).IsRequired().HasMaxLength(50);
            builder.Property(x => x.NombreAseguradora).IsRequired().HasMaxLength(50);
            builder.Property(x => x.NumeroPoliza).IsRequired();
            builder.Property(x => x.FechaInicio).IsRequired();
            builder.Property(x => x.FechaVencimiento).IsRequired();
            builder.Property(x => x.Estado).IsRequired();

        }
    }
}