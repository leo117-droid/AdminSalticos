using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdmin.Modelos.ViewModels
{
    public class PersonalVM
    {

        public Personal Personal { get; set; }

        public IEnumerable<SelectListItem> RolPersonalLista { get; set; }
    }
}
