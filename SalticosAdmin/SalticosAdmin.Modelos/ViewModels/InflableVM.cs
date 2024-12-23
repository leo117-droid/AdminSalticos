using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdmin.Modelos.ViewModels
{
    public class InflableVM
    {

        public Inflable Inflable { get; set; }

        public IEnumerable<SelectListItem> CategoriaTamannoLista  { get; set; }
        public IEnumerable<SelectListItem> CategoriaEdadLista { get; set; }

        public IEnumerable<Inflable> InflableLista { get; set; }

    }
}
