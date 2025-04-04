﻿using SalticosAdmin.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio
{
    public interface IMobiliarioRepositorio : IRepositorio<Mobiliario>
    {
        void Actualizar(Mobiliario mobiliario);

        Task<List<Mobiliario>> ObtenerMobiliariosSolapados(DateTime fechaEvento, TimeSpan horaInicio, TimeSpan horaFin);
    }
}
