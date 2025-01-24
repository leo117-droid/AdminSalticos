using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdmin.Modelos
{
    public class EventoServicioAdicional
    {
        [Key]
        public int Id { get; set; }

        public int IdServicioAdicional { get; set; }

        [ForeignKey("IdServicioAdicional")]
        public ServicioAdicional ServicioAdicional { get; set; }

        public int IdEvento { get; set; }

        [ForeignKey("IdEvento")]
        public Evento Evento { get; set; }

        [Required(ErrorMessage = "Cantidad es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Cantidad { get; set; }

        [NotMapped] 
        public string? NombreServicioAdicional { get; set; }

    }
}
