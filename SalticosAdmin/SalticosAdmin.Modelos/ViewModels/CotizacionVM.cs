namespace SalticosAdmin.Modelos.ViewModels
{

    public class CotizacionVM
    {
        public List<Inflable> Inflables { get; set; }
        public List<Mobiliario> Mobiliarios { get; set; }
        public List<ServicioAdicional> ServiciosAdicionales { get; set; }
        public List<Alimentacion> Alimentacion { get; set; } = new List<Alimentacion>();


        public List<Inflable> InflablesSeleccionados { get; set; }
        public List<(Mobiliario Mobiliario, int Cantidad)> MobiliariosSeleccionados { get; set; }
        public List<(ServicioAdicional Servicio, int Cantidad)> ServiciosSeleccionados { get; set; }
        public List<(Alimentacion Alimentacion, int Cantidad)> AlimentacionSeleccionada { get; set; } // Nueva propiedad

        public double MontoTotal { get; set; }
    }
}