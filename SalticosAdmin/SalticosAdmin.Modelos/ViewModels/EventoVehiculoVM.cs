using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdmin.Modelos.ViewModels
{
    public class EventoVehiculoVM
    {

        public int IdEvento { get; set; }

        public int IdRelacion { get; set; }

        public int IdVehiculo { get; set; }

        public string VehiculoNombre { get; set; }


        public IEnumerable<SelectListItem> ListaVehiculo { get; set; }


    }
}
