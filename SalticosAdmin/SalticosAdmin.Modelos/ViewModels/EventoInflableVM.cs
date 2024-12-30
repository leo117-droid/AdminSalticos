using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdmin.Modelos.ViewModels
{
    public class EventoInflableVM
    {

        public int IdEvento { get; set; }

        public int IdRelacion { get; set; }

        public int IdInflable { get; set; }

        public string InflableNombre { get; set; }


        public IEnumerable<SelectListItem> ListaInflable { get; set; }


    }
}
