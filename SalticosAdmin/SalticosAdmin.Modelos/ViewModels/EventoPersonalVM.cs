using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdmin.Modelos.ViewModels
{
    public class EventoPersonalVM
    {

        public int IdEvento { get; set; }

        public int IdRelacion { get; set; }

        public int IdPersonal { get; set; }

        public string PersonalNombre { get; set; }


        public IEnumerable<SelectListItem> ListaPersonal { get; set; }


    }
}
