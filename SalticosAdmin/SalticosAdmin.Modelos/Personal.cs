using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdmin.Modelos
{
    public class Personal
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nombre es requerido")]
        [MaxLength(50, ErrorMessage = "Nombre debe ser maximo 50 caracteres")]
        public string Nombre { get; set; }

        [MaxLength(50, ErrorMessage = "Apellidos debe ser maximo 50 caracteres")]
        public string Apellidos { get; set; }

        [Required(ErrorMessage = "Telefono es requerido")]
        [StringLength(8, ErrorMessage = "La cedula debe tener exactamente 8 caracteres", MinimumLength = 8)]
        [RegularExpression(@"^\d+$", ErrorMessage = "El Telefono solo debe contener números")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "Correo es requerido")]
        [MaxLength(50, ErrorMessage = "Correo debe ser máximo 50 caracteres")]
        [EmailAddress(ErrorMessage = "El formato del correo no es válido")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "Cedula es requerido")]
        [MaxLength(11, ErrorMessage = "Cedula debe ser máximo 11 dígitos")]
        [RegularExpression(@"^\d+$", ErrorMessage = "La Cedula solo debe contener números")]
        public string Cedula { get; set; }

        [Required(ErrorMessage = "Fecha de Nacimiento es requerida")]
        [DataType(DataType.Date, ErrorMessage = "Formato de fecha no válido")]
        public DateTime FechaNacimiento { get; set; }

        [Required(ErrorMessage = "Fecha de Entrada es requerida")]
        [DataType(DataType.Date, ErrorMessage = "Formato de fecha no válido")]
        public DateTime FechaEntrada { get; set; }

        [Required(ErrorMessage = "Rol Personal es Requerido")]
        public int RolPersonalId { get; set; }

        [ForeignKey("RolPersonalId")]
        public RolPersonal RolPersonal { get; set; }
    }
}
