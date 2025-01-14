using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SalticosAdmin.Modelos
{
    public class Cotizacion
    {
        [Key]
        public int Id { get; set; }

        public List<int> InflableIds { get; set; } = new List<int>();
        public List<Inflable> InflablesSeleccionados { get; set; }

        public List<(int MobiliarioId, int Cantidad)> MobiliariosSeleccionados { get; set; }
            = new List<(int MobiliarioId, int Cantidad)>();

        [Required]
        public double MontoTotal { get; set; }
    }
}
