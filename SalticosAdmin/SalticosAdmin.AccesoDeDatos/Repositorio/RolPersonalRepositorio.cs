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
    public class RolPersonalRepositorio : Repositorio<RolPersonal>, IRolPersonalRepositorio
    {
        private readonly ApplicationDbContext _db;

        public RolPersonalRepositorio(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Actualizar(RolPersonal rolPersonal)
        {
            var rolPersonalBD = _db.RolPersonal.FirstOrDefault(b => b.Id == rolPersonal.Id);
            if (rolPersonalBD != null)
            {
                rolPersonalBD.Nombre = rolPersonal.Nombre;
                _db.SaveChanges();

            }
        }

    }
}
