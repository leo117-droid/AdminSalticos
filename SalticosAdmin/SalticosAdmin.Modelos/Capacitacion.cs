﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdmin.Modelos
{
    public class Capacitacion
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Fecha es requerida")]
        [DataType(DataType.Date, ErrorMessage = "Formato de fecha no válido")]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "Tema es requerido")]
        [MaxLength(50, ErrorMessage = "Tema debe ser máximo 50 caracteres")]
        public string Tema { get; set; }

        [Required(ErrorMessage = "Duración es requerido")]
        [MaxLength(50, ErrorMessage = "Duración debe ser máximo 50 caracteres")]
        public string Duracion { get; set; }

    }
}
