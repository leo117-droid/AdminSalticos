using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdmin.Modelos
{
    public class Seguro
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tipo del seguro es requerido")]
        [MaxLength(50, ErrorMessage = "Tipo del seguro debe ser maximo 50 caracteres")]
        public string TipoSeguro { get; set; }

        [Required(ErrorMessage = "Nombre Aseguradora es requerido")]
        [MaxLength(50, ErrorMessage = "Nombre Aseguradora debe ser maximo 50 caracteres")]
        public string NombreAseguradora { get; set; }

        [Required(ErrorMessage = "NumeroPoliza es requerido")]
        [Range(0, int.MaxValue, ErrorMessage = "Numero Poliza no puede ser negativo.")]
        public int NumeroPoliza { get; set; }

        [Required(ErrorMessage = "Fecha Inicio es requerida")]
        [DataType(DataType.Date, ErrorMessage = "Formato de fecha no válido")]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "Fecha Vencimiento es requerida")]
        [DataType(DataType.Date, ErrorMessage = "Formato de fecha no válido")]
        public DateTime FechaVencimiento { get; set; }


        [Required(ErrorMessage = "Estado es Requerido")]
        public bool Estado { get; set; }

    }
}
