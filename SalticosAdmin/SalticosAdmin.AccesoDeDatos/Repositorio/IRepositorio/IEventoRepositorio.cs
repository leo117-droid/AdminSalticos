using Microsoft.AspNetCore.Mvc.Rendering;
using SalticosAdmin.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio
{
    public interface IEventoRepositorio : IRepositorio<Evento>
    {
        void Actualizar(Evento evento);

        IEnumerable<SelectListItem> ObtenerTodosDropdownLista(string obj);

        Task<List<Evento>> ObtenerEventosSolapados(DateTime fecha, TimeSpan horaInicio, TimeSpan horaFin);
    }
}