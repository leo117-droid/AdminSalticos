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
    public class CapacitacionPersonalRepositorio : Repositorio<CapacitacionPersonal>, ICapacitacionPersonalRepositorio
    {
        private readonly ApplicationDbContext _db;

        public CapacitacionPersonalRepositorio(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Actualizar(CapacitacionPersonal capacitacionPersonal)
        {
            var capacitacionPersonalBD = _db.CapacitacionesPersonal.FirstOrDefault(b => b.Id == capacitacionPersonal.Id);
            if (capacitacionPersonalBD != null)
            {
                capacitacionPersonalBD.IdPersonal = capacitacionPersonal.IdPersonal;
                capacitacionPersonalBD.IdCapacitacion = capacitacionPersonal.IdCapacitacion;

                _db.SaveChanges();

            }
        }



        public IEnumerable<SelectListItem> ObtenerPersonal(string obj, int? idCapacitacion)
        {
            if (obj.Equals("Personal")){
                var personal = _db.Personal
                                .Where(t => !_db.CapacitacionesPersonal
                                            .Any(cp => cp.IdPersonal == t.Id && cp.IdCapacitacion == idCapacitacion))
                                .Select(c => new SelectListItem
                                {
                                    Text = $"{c.Nombre} {c.Apellidos} - {c.Cedula}",
                                    Value = c.Id.ToString()
                                })
                                .ToList();
                return personal;
            }
            return null;
        }

    }
}

