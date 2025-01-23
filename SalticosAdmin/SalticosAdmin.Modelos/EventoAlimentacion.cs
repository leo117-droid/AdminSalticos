using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdmin.Modelos
{
    public class EventoAlimentacion
    {
        [Key]
        public int Id { get; set; }
       
        [Required(ErrorMessage = "Alimentación es requerida")]

        public int IdAlimentacion { get; set; }

        [ForeignKey("IdAlimentacion")]
        [Required(ErrorMessage = "Alimentación es requerida")]

        public Alimentacion Alimentacion { get; set; }

        public int IdEvento { get; set; }

        [ForeignKey("IdEvento")]
        public Evento Evento { get; set; }

        [Required(ErrorMessage = "Cantidad es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Cantidad { get; set; }

        [NotMapped] 
        public string? NombreIngrediente { get; set; }
    }
}
