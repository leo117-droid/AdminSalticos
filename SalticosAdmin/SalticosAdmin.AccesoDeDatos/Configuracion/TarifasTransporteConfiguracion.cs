using Microsoft.EntityFrameworkCore;
using SalticosAdmin.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SalticosAdmin.Modelos;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SalticosAdmin.AccesoDeDatos.Configuracion
{
    public class TarifasTransporteConfiguracion : IEntityTypeConfiguration<TarifasTransporte>
    {
        public void Configure(EntityTypeBuilder<TarifasTransporte> builder)
        {
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.Provincia).IsRequired().HasMaxLength(20);
            builder.Property(x => x.Precio).IsRequired();

        }
    }
}
