using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdmin.Modelos
{
    public class Usuario : IdentityUser
    {


        [Required(ErrorMessage = "Nombre es requerido")]
        [MaxLength(45, ErrorMessage = "El nombre debe tener máximo 45 caracteres")]
        public string Nombre { get; set; }


        [Required(ErrorMessage = "Apellido es requerido")]
        [MaxLength(45, ErrorMessage = "El apellido debe tener máximo 45 caracteres")]
        public string Apellido { get; set; }

    }
}
