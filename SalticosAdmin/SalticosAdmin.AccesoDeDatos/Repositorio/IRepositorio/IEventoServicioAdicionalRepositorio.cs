using Microsoft.AspNetCore.Mvc.Rendering;
using SalticosAdmin.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio
{
    public interface IEventoServicioAdicionalRepositorio : IRepositorio<EventoServicioAdicional>
    {
        void Actualizar(EventoServicioAdicional eventoServicioAdicional);


        IEnumerable<SelectListItem> ObtenerServicio(string objeto, int? idEvento);

    }
}
