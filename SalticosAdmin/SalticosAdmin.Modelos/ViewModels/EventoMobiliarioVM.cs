using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdmin.Modelos.ViewModels
{
    public class EventoMobiliarioVM
    {

        public int IdEvento { get; set; }

        public int IdRelacion { get; set; }

        public int IdMobiliario { get; set; }

        public string MobiliarioNombre { get; set; }
        public int Cantidad { get; set; }


        public IEnumerable<SelectListItem> ListaMobiliario { get; set; }


    }
}
