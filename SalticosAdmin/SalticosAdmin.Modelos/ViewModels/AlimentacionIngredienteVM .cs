using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdmin.Modelos.ViewModels
{
    public class AlimentacionIngredienteVM
    {

        public int IdAlimentacion { get; set; }

        public int IdRelacion { get; set; }

        public int IdIngrediente { get; set; }

        public string IngredienteNombre { get; set; }

        public IEnumerable<SelectListItem> ListaIngrediente { get; set; }


    }
}
