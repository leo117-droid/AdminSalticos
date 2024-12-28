using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdmin.Modelos.ViewModels
{
    public class EventoAlimentacionVM
    {

        public int IdEvento { get; set; }

        public int IdRelacion { get; set; }

        public int IdAlimentacion { get; set; }

        public string AlimentacionNombre { get; set; }
        public int Cantidad { get; set; }


        public IEnumerable<SelectListItem> ListaAlimentacion { get; set; }


    }
}
