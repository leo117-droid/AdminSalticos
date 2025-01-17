using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdmin.Modelos
{
    public class Proveedor
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de la empresa es requerido")]
        [MaxLength(100, ErrorMessage = "El nombre de la empresa debe tener un máximo de 100 caracteres")]
        public string NombreEmpresa { get; set; }

        [Required(ErrorMessage = "El nombre de contacto es requerido")]
        [MaxLength(50, ErrorMessage = "El nombre de contacto debe tener un máximo de 50 caracteres")]
        public string Contacto { get; set; }

        [Required(ErrorMessage = "El teléfono es requerido")]
        [Phone(ErrorMessage = "Formato de teléfono no válido")]
        [MaxLength(15, ErrorMessage = "El teléfono debe tener un máximo de 15 caracteres")]
        [RegularExpression(@"^\d+$", ErrorMessage = "El Telefono solo debe contener números")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "El correo electrónico es requerido")]
        [EmailAddress(ErrorMessage = "Formato de correo no válido")]
        [MaxLength(50, ErrorMessage = "El correo electrónico debe tener un máximo de 50 caracteres")]
        public string Correo { get; set; }


        [Required(ErrorMessage = "La dirección es requerida")]
        [MaxLength(200, ErrorMessage = "La dirección debe tener un máximo de 200 caracteres")]
        public string Direccion { get; set; }

        [Required(ErrorMessage = "La descripción es requerida")]
        [MaxLength(300, ErrorMessage = "La descripción debe tener un máximo de 300 caracteres")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El tipo de proveedor es requerido")]
        [MaxLength(50, ErrorMessage = "El tipo de proveedor debe tener un máximo de 50 caracteres")]
        public string TipoProveedor { get; set; }
    }
}
