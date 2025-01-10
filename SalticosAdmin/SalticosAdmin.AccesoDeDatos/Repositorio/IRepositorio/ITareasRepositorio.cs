using SalticosAdmin.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio
{
    public interface ITareasRepositorio : IRepositorio<Tareas>
    {
        void Actualizar(Tareas tarea);
        IEnumerable<SelectListItem> ObtenerTodosDropdownLista(string obj);
        Task<IEnumerable<Tareas>> FiltrarPorEstado(string estado);
        Task<IEnumerable<Tareas>> FiltrarPorPrioridad(string prioridad);
    }
}
