using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdmin.Modelos
{
    public class Inflable
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nombre es requerido")]
        [MaxLength(50, ErrorMessage = "Nombre debe ser maximo 50 caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Descripcion es requerido")]
        [MaxLength(100, ErrorMessage = "Descripcion debe ser maximo 100 caracteres")]
        public string Descripcion { get; set; }


        [Required(ErrorMessage = "Dimensiones es requerido")]
        [MaxLength(100, ErrorMessage = "La Dimensiones debe ser maximo 100 caracteres")]
        public string Dimensiones { get; set; }


        [Required(ErrorMessage = "Estado es Requerido")]
        public bool Estado { get; set; }

        public string ImageUrl { get; set; }

        [Required(ErrorMessage = "Precio es requerido")]
        public double Precio { get; set; }

        [Required(ErrorMessage = "Precio por hora adicional es requerido")]
        public double PrecioHoraAdicional { get; set; }

        [Required(ErrorMessage = "Categoria Tamanno es Requerido")]
        public int CategoriaTamannoId { get; set; }


        [ForeignKey("CategoriaTamannoId")]
        public CategoriaTamanno CategoriaTamanno { get; set; }


        [Required(ErrorMessage = "Categoria Edad es Requerido")]
        public int CategoriaEdadId { get; set; }

        [ForeignKey("CategoriaEdadId")]
        public CategoriasEdad CategoriasEdad { get; set; }

        public int? PadreId { get; set; }

        public virtual Inflable Padre { get; set; }
    }
}
