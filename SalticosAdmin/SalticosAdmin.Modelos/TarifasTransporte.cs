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

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var provinciasValidas = new List<string>
            {
                "San Jose", "Alajuela", "Cartago", "Heredia", "Guanacaste", "Puntarenas", "Limon"
            };

            // Normalizar la provincia ingresada
            string provinciaNormalizada = RemoverTildes(Provincia).ToLower();

            // Normalizar las provincias válidas y comparar
            var provinciasNormalizadas = provinciasValidas
                .Select(p => RemoverTildes(p).ToLower())
                .ToList();

            if (!provinciasNormalizadas.Contains(provinciaNormalizada))
            {
                yield return new ValidationResult(
                    "La provincia ingresada no es válida. Debe ser una de las provincias de Costa Rica.",
                    new[] { nameof(Provincia) });
            }
        }

        private static string RemoverTildes(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return texto;

            var textoNormalizado = texto.Normalize(NormalizationForm.FormD);
            var resultado = new string(textoNormalizado
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray());
            return resultado.Normalize(NormalizationForm.FormC);
        }
    }
}
