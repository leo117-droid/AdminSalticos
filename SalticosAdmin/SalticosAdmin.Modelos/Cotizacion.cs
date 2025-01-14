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


        [Required]
        public double MontoTotal { get; set; }
    }
}
