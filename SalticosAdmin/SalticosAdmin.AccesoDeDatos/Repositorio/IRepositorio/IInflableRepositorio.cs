using Microsoft.AspNetCore.Mvc.Rendering;
using SalticosAdmin.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio
{
    public interface IInflableRepositorio : IRepositorio<Inflable>
    {
        void Actualizar(Inflable inflable);

        IEnumerable<SelectListItem> ObtenerTodosDropdownLista(string obj);

        Task<IEnumerable<Inflable>> FiltrarPorCategorias(int? categoriaTamannoId, int? categoriaEdadId);
    }
}