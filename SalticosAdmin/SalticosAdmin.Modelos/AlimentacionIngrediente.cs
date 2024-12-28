using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdmin.Modelos
{
    public class AlimentacionIngrediente
    {
        [Key]
        public int Id { get; set; }

        public int IdIngrediente { get; set; }

        [ForeignKey("IdIngrediente")]
        public Ingrediente Ingrediente { get; set; }

        public int IdAlimentacion { get; set; }

        [ForeignKey("IdAlimentacion")]
        public Alimentacion Alimentacion { get; set; }

        [NotMapped] // Nose agrega a la base de datos
        public string? NombreIngrediente { get; set; }
    }
}
