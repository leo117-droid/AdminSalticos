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
    public class CategoriaTamannoRepositorio : Repositorio<CategoriaTamanno>, ICategoriaTamannoRepositorio
    {
        private readonly ApplicationDbContext _db;

        public CategoriaTamannoRepositorio(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Actualizar(CategoriaTamanno categoriaTamanno)
        {
            var categoriaTamannoBD = _db.CategoriaTammano.FirstOrDefault(b => b.Id == categoriaTamanno.Id);
            if (categoriaTamannoBD != null)
            {
                categoriaTamannoBD.Nombre = categoriaTamanno.Nombre;
                _db.SaveChanges();

            }
        }

    }
}
