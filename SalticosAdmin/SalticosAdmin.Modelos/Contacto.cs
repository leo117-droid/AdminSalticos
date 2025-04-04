﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdmin.Modelos
{
    public class Contacto
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nombre es requerido")]
        [MaxLength(50, ErrorMessage = "Nombre debe ser máximo 50 caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Apellido es requerido")]
        [MaxLength(50, ErrorMessage = "Apellido debe ser máximo 50 caracteres")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "Tipo Servicio es requerido")]
        [MaxLength(50, ErrorMessage = "Tipo Servicio debe ser maximo 50 caracteres")]
        public string TipoServicio { get; set; }

        [Required(ErrorMessage = "Dirección es requerido")]
        [MaxLength(100, ErrorMessage = "Dirección debe ser máximo 100 caracteres")]
        public string Direccion { get; set; }

        [Required(ErrorMessage = "Teléfono es requerido")]
        [StringLength(8, ErrorMessage = "El teléfono debe tener exactamente 8 caracteres", MinimumLength = 8)]
        [RegularExpression(@"^\d+$", ErrorMessage = "El teléfono solo debe contener números")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "Correo es requerido")]
        [MaxLength(50, ErrorMessage = "Correo debe ser máximo 50 caracteres")]
        [EmailAddress(ErrorMessage = "El formato del correo no es válido")]
        public string Correo { get; set; }

    }
}
