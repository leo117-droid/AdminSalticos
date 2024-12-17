using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdmin.Modelos
{
    public class Vehiculo
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Marca es requerida")]
        [MaxLength(50, ErrorMessage = "Marca debe ser maximo 50 caracteres")]
        public string Marca { get; set; }

        [Required(ErrorMessage = "Modelo es requerido")]
        [MaxLength(50, ErrorMessage = "Modelo debe ser maximo 50 caracteres")]
        public string Modelo { get; set; }

        [Required(ErrorMessage = "Placa es requerida")]
        [MaxLength(20, ErrorMessage = "Placa debe ser maximo 20 caracteres")]
        public string Placa { get; set; }

        [Required(ErrorMessage = "Tipo de vehiculo es requerido")]
        [MaxLength(50, ErrorMessage = "Tipo de vehiculo debe ser maximo 50 caracteres")]
        public string TipoVehiculo { get; set; }


    }
}
