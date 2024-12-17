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
    public class VehiculoConfiguracion : IEntityTypeConfiguration<Vehiculo>
    {
        public void Configure(EntityTypeBuilder<Vehiculo> builder)
        {
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.Marca).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Modelo).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Placa).IsRequired().HasMaxLength(20);
            builder.Property(x => x.TipoVehiculo).IsRequired().HasMaxLength(50);



        }
    }
}