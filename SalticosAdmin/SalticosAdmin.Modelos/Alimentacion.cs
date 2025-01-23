using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdmin.Modelos
{
    public class Alimentacion
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nombre es requerido")]
        [MaxLength(50, ErrorMessage = "Nombre debe ser máximo 50 caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Descripción es requerido")]
        [MaxLength(100, ErrorMessage = "Descripción debe ser máximo 100 caracteres")]
        public string Descripcion { get; set; }

        public string ImageUrl { get; set; }

        [Required(ErrorMessage = "Precio es requerido")]
        [Range(0, int.MaxValue, ErrorMessage = "El precio no puede ser negativo.")]

        public double Precio { get; set; }

    }
}
