﻿using Microsoft.EntityFrameworkCore;
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
    public class MobiliarioConfiguracion : IEntityTypeConfiguration<Mobiliario>
    {
        public void Configure(EntityTypeBuilder<Mobiliario> builder)
        {
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.Nombre).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Descripcion).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Dimensiones).IsRequired().HasMaxLength(80);
            builder.Property(x => x.ImageUrl).IsRequired(false);
            builder.Property(x => x.Inventario).IsRequired();
            builder.Property(x => x.Precio).IsRequired();


        }
    }
}
