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
    public class IngredienteRepositorio : Repositorio<Ingrediente>, IIngredienteRepositorio
    {
        private readonly ApplicationDbContext _db;

        public IngredienteRepositorio(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Actualizar(Ingrediente ingrediente)
        {
            var ingredienteBD = _db.Ingredientes.FirstOrDefault(b => b.Id == ingrediente.Id);
            if (ingredienteBD != null)
            {
                ingredienteBD.Nombre = ingrediente.Nombre;
                ingredienteBD.Descripcion = ingrediente.Descripcion;
                ingredienteBD.Precio = ingrediente.Precio;
                _db.SaveChanges();

            }
        }

    }
}
