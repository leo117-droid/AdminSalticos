using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdmin.Modelos
{
    public class Bitacora
    {
        [Key]
        public int Id { get; set; }

        public DateTime Fecha { get; set; }
        
        public TimeSpan Hora { get; set; }

        [Required(ErrorMessage = "Acción es requerido")]
        [MaxLength(200, ErrorMessage = "Acción debe ser máximo 200 caracteres")]
        public string Accion { get; set; }
        public string Usuario { get; set; }


    }
}
