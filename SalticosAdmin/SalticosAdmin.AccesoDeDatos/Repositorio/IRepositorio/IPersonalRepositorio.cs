﻿using Microsoft.AspNetCore.Mvc.Rendering;
using SalticosAdmin.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio
{
    public interface IPersonalRepositorio : IRepositorio<Personal>
    {
        void Actualizar(Personal personal);

        IEnumerable<SelectListItem> ObtenerTodosDropdownLista(string obj);
        Task<IEnumerable<Personal>> FiltrarPorRolPersonal(int value);



    }
}