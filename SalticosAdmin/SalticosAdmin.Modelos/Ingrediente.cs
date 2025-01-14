using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdmin.Modelos
{
    public class Ingrediente
    {

        [Key]
        public int Id { get; set; }


        [Required(ErrorMessage = "Nombre es requerido")]
        [MaxLength(50, ErrorMessage = "Nombre debe ser maximo 50 caracteres")]
        public string Nombre { get; set; }


        [Required(ErrorMessage = "Descripción es requerido")]
        [MaxLength(100, ErrorMessage = "Descripción debe ser maximo 100 caracteres")]
        public string Descripcion { get; set; }


        [Required(ErrorMessage = "Precio es requerido")]
        [Range(0, int.MaxValue, ErrorMessage = "El precio no puede ser negativo.")]
        public double Precio { get; set; }
    }
}
