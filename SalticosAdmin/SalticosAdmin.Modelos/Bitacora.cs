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

        [Required(ErrorMessage = "Accion es requerido")]
        [MaxLength(100, ErrorMessage = "Accion debe ser maximo 100 caracteres")]
        public string Accion { get; set; }
        public string Usuario { get; set; }


    }
}
