using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SalticosAdmin.Modelos
{
    public class TarifasTransporte : IValidatableObject
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de la provincia es requerido")]
        [MaxLength(20, ErrorMessage = "El nombre de la provincia debe tener un máximo de 20 caracteres")]
        public string Provincia { get; set; }

        [Required(ErrorMessage = "El precio es requerido")]
        [Range(0, double.MaxValue, ErrorMessage = "El precio debe ser un valor positivo")]
        public double Precio { get; set; }

        
    }
}
