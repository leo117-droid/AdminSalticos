using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdmin.Modelos
{
    public class Evento
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Fecha es requerida")]
        [DataType(DataType.Date, ErrorMessage = "Formato de fecha no válido")]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "Hora Inicio es requerida")]
        [DataType(DataType.Time, ErrorMessage = "Formato de hora no válido")]
        public TimeSpan HoraInicio { get; set; }

        [Required(ErrorMessage = "Hora Final es requerida")]
        [DataType(DataType.Time, ErrorMessage = "Formato de hora no válido")]
        public TimeSpan HoraFinal { get; set; }


        [Required(ErrorMessage = "Direccion es requerido")]
        [MaxLength(120, ErrorMessage = "Direccion debe ser maximo 120 caracteres")]
        public string Direccion { get; set; }

        [Required(ErrorMessage = "El nombre de la provincia es requerido")]
        [MaxLength(20, ErrorMessage = "El nombre de la provincia debe tener un máximo de 20 caracteres")]
        public string Provincia { get; set; }

        [Required(ErrorMessage = "Cliente es Requerido")]
        public int ClienteId { get; set; }


        [ForeignKey("ClienteId")]
        public Cliente Cliente { get; set; }

    }
}
