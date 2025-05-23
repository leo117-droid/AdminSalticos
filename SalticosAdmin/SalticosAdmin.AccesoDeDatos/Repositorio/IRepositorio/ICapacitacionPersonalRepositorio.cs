﻿using Microsoft.AspNetCore.Mvc.Rendering;
using SalticosAdmin.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio
{
    public interface ICapacitacionPersonalRepositorio : IRepositorio<CapacitacionPersonal>
    {
        void Actualizar(CapacitacionPersonal capacitacionPersonal);


        IEnumerable<SelectListItem> ObtenerPersonal(string objeto, int? idCapacitacion);

    }
}
