using Microsoft.AspNetCore.Mvc.Rendering;
using SalticosAdmin.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio
{
    public interface IEventoPersonalRepositorio : IRepositorio<EventoPersonal>
    {
        void Actualizar(EventoPersonal eventoPersonal);


        IEnumerable<SelectListItem> ObtenerPersonal(string objeto, int? idEvento);

    }
}
