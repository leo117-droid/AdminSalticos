using SalticosAdmin.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio
{
    public interface ITareaRepositorio : IRepositorio<Tarea>
    {
        void Actualizar(Tarea tarea);
        IEnumerable<SelectListItem> ObtenerTodosDropdownLista(string obj);
        Task<IEnumerable<Tarea>> FiltrarPorEstado(string estado);
        Task<IEnumerable<Tarea>> FiltrarPorPrioridad(string prioridad);
    }
}
