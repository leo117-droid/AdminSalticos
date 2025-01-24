using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdmin.Modelos
{
    public class EventoInflable
    {
        [Key]
        public int Id { get; set; }

        public int IdInflable { get; set; }

        [ForeignKey("IdInflable")]
        public Inflable Inflable { get; set; }

        public int IdEvento { get; set; }

        [ForeignKey("IdEvento")]
        public Evento Evento { get; set; }


        [NotMapped]
        public string? NombreInflable{ get; set; }

    }
}
