using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdmin.Modelos
{
    public class Tarea
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Título es requerido")]
        [MaxLength(100, ErrorMessage = "Título debe ser máximo 100 caracteres")]
        public string Titulo { get; set; }
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "Prioridad es requerida")]
        [MaxLength(20, ErrorMessage = "Prioridad debe ser máximo 20 caracteres")]
        public string Prioridad { get; set; }

        public DateTime Fecha { get; set; }

        public TimeSpan Hora { get; set; }

        [Required(ErrorMessage = "Estado es requerido")]
        [MaxLength(20, ErrorMessage = "Estado debe ser máximo 20 caracteres")]
        public string Estado { get; set; }

    }
}
