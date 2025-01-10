using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdmin.Modelos
{
    public class Tareas
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Titulo es requerido")]
        [MaxLength(100, ErrorMessage = "Titulo debe ser maximo 100 caracteres")]
        public string Titulo { get; set; }
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "Prioridad es requerida")]
        [MaxLength(20, ErrorMessage = "Prioridad debe ser maximo 20 caracteres")]
        public string Prioridad { get; set; }

        public DateTime Fecha { get; set; }

        public TimeSpan Hora { get; set; }

        [Required(ErrorMessage = "Estado es requerido")]
        [MaxLength(20, ErrorMessage = "Estado debe ser maximo 20 caracteres")]
        public string Estado { get; set; }

    }
}
