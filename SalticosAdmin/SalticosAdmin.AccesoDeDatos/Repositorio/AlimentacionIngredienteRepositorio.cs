using Microsoft.AspNetCore.Mvc.Rendering;
using SalticosAdmin.AccesoDeDatos.Data;
using SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio;
using SalticosAdmin.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SalticosAdmin.AccesoDeDatos.Repositorio
{
    public class AlimentacionIngredienteRepositorio : Repositorio<AlimentacionIngrediente>, IAlimentacionIngredienteRepositorio
    {
        private readonly ApplicationDbContext _db;

        public AlimentacionIngredienteRepositorio(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Actualizar(AlimentacionIngrediente alimentacionIngrediente)
        {
            var alimentacionIngredienteBD = _db.AlimentacionIngredientes.FirstOrDefault(b => b.Id == alimentacionIngrediente.Id);
            if (alimentacionIngredienteBD != null)
            {
                alimentacionIngredienteBD.IdIngrediente = alimentacionIngrediente.IdIngrediente;
                alimentacionIngredienteBD.IdAlimentacion = alimentacionIngrediente.IdAlimentacion;

                _db.SaveChanges();

            }
        }

        public IEnumerable<SelectListItem> ObtenerIngrediente(string obj, int? idAlimentacion)
        {
            if (obj.Equals("Ingrediente")){ // o Ingredientes 
                var ingrediente = _db.Ingredientes
                                .Where(t => !_db.AlimentacionIngredientes
                                            .Any(cp => cp.IdIngrediente == t.Id && cp.IdAlimentacion == idAlimentacion))
                                .Select(c => new SelectListItem
                                {
                                    Text = $"{c.Nombre}",
                                    Value = c.Id.ToString()
                                })
                                .ToList();
                return ingrediente;
            }
            return null;
        }

    }
}

