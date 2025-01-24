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
        [MaxLength(50, ErrorMessage = "Marca debe ser máximo 50 caracteres")]
        public string Marca { get; set; }

        [Required(ErrorMessage = "Modelo es requerido")]
        [MaxLength(50, ErrorMessage = "Modelo debe ser máximo 50 caracteres")]
        public string Modelo { get; set; }

        [Required(ErrorMessage = "Placa es requerida")]
        [MaxLength(6, ErrorMessage = "Placa debe ser máximo 6 caracteres")]
        [StringLength(6, ErrorMessage = "La placa debe tener exactamente 6 caracteres", MinimumLength = 6)]

        public string Placa { get; set; }

        [Required(ErrorMessage = "Tipo de vehículo es requerido")]
        [MaxLength(50, ErrorMessage = "Tipo de vehículo debe ser máximo 50 caracteres")]
        public string TipoVehiculo { get; set; }


    }
}
