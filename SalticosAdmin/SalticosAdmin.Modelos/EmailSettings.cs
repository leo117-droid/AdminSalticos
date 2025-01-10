using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdmin.Modelos
{
    public class EmailSettings
    {
        public string APIKey { get; set; }
        public string SenderEmail { get; set; }
        public string SenderName { get; set; }

    }
}
