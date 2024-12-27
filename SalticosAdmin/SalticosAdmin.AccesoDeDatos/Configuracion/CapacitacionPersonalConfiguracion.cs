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
    public class CapacitacionPersonalConfiguracion : IEntityTypeConfiguration<CapacitacionPersonal>
    {
        public void Configure(EntityTypeBuilder<CapacitacionPersonal> builder)
        {
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.IdPersonal).IsRequired();
            builder.Property(x => x.IdCapacitacion).IsRequired();


            builder.HasOne(x => x.Personal).WithMany()
                .HasForeignKey(x => x.IdPersonal).
                OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.Capacitacion).WithMany()
                .HasForeignKey(x => x.IdCapacitacion).
                OnDelete(DeleteBehavior.NoAction);

        }
    }
}