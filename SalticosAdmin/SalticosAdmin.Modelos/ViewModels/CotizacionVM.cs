using System.Collections.Generic;
using SalticosAdmin.Modelos;

namespace SalticosAdmin.Modelos.ViewModels
{
    public class CotizacionVM
    {
        public List<Inflable> Inflables { get; set; }
        public List<Mobiliario> Mobiliarios { get; set; }
        public List<ServicioAdicional> ServiciosAdicionales { get; set; }
        public List<Inflable> InflablesSeleccionados { get; set; }
        public List<(Mobiliario Mobiliario, int Cantidad)> MobiliariosSeleccionados { get; set; }
        public List<(ServicioAdicional Servicio, int Cantidad)> ServiciosSeleccionados { get; set; } // Servicios con cantidad
        public double MontoTotal { get; set; }
    }


}
