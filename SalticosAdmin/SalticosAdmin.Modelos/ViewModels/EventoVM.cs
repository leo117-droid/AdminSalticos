using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdmin.Modelos.ViewModels
{
    public class EventoVM
    {

        public Evento Evento { get; set; }

        public IEnumerable<SelectListItem> ClienteLista  { get; set; }

        public IEnumerable<Evento> EventoLista { get; set; }

    }
}
