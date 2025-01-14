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
    public class TareaConfiguracion : IEntityTypeConfiguration<Tareas>
    {
        public void Configure(EntityTypeBuilder<Tareas> builder)
        {

            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.Titulo).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Descripcion).HasMaxLength(500); //Limite opcional, se puede cambiar si se desea
            builder.Property(x => x.Prioridad).IsRequired().HasMaxLength(20);
            builder.Property(x => x.Fecha).IsRequired();
            builder.Property(x => x.Hora).IsRequired(); //Fecha y hora los pongo obligatorios por si acaso 
            builder.Property(x => x.Estado).IsRequired().HasMaxLength(20);
        }
    }
}

