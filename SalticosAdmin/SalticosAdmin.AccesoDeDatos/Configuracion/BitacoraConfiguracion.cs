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
    public class BitacoraConfiguracion : IEntityTypeConfiguration<Bitacora>
    {
        public void Configure(EntityTypeBuilder<Bitacora> builder)
        {
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.Fecha);
            builder.Property(x => x.Hora);
            builder.Property(x => x.Accion).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Usuario);

        }
    }
}
