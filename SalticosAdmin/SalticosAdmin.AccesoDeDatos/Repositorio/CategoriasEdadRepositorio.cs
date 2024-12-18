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
    public class CategoriasEdadRepositorio : Repositorio<CategoriasEdad>, ICategoriasEdadRepositorio
    {
        private readonly ApplicationDbContext _db;

        public CategoriasEdadRepositorio(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Actualizar(CategoriasEdad categoriaEdad)
        {
            var categoriasEdadBD = _db.CategoriasEdades.FirstOrDefault(b => b.Id == categoriaEdad.Id);
            if (categoriasEdadBD != null)
            {
                categoriasEdadBD.Nombre = categoriaEdad.Nombre;
                _db.SaveChanges();

            }
        }

    }
}
