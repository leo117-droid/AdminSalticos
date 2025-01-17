using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdmin.Modelos
{
    public class InformeInflable
    {
        [Key]
        public int IdReserva { get; set; }

        [Required]
        public int IdInflable { get; set; }

        [Required]
        public DateTime FechaReserva { get; set; }

        [ForeignKey("IdInflable")]
        public Inflable Inflable { get; set; }
    }
}
