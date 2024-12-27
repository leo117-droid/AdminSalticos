using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdmin.Modelos
{
    public class CapacitacionPersonal
    {
        [Key]
        public int Id { get; set; }

        public int IdPersonal { get; set; }

        [ForeignKey("IdPersonal")]
        public Personal Personal { get; set; }

        public int IdCapacitacion { get; set; }

        [ForeignKey("IdCapacitacion")]
        public Capacitacion Capacitacion { get; set; }

        [NotMapped] // Nose agrega a la base de datos
        public string? NombrePersonal { get; set; }
    }
}
