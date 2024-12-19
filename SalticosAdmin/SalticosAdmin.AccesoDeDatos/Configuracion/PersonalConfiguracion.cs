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
    public class PersonalConfiguracion : IEntityTypeConfiguration<Personal>
    {
        public void Configure(EntityTypeBuilder<Personal> builder)
        {
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.Nombre).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Apellidos).HasMaxLength(50);
            builder.Property(x => x.Telefono).IsRequired().HasMaxLength(8);
            builder.Property(x => x.Correo).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Cedula).IsRequired().HasMaxLength(11);
            builder.Property(x => x.FechaNacimiento).IsRequired().HasColumnType("date");
            builder.Property(x => x.FechaEntrada).IsRequired().HasColumnType("date");

            //Relaciones
            builder.HasOne(x => x.RolPersonal).WithMany().HasForeignKey(x => x.RolPersonalId).OnDelete(DeleteBehavior.NoAction);

        }
    }
}