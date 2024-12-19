using SalticosAdmin.AccesoDeDatos.Data;
using SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio;
using SalticosAdmin.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SalticosAdmin.AccesoDeDatos.Repositorio
{
    public class PersonalRepositorio : Repositorio<Personal>, IPersonalRepositorio
    {
        private readonly ApplicationDbContext _db;

        public PersonalRepositorio(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Actualizar(Personal personal)
        {
            var personalBD = _db.Personal.FirstOrDefault(b => b.Id == personal.Id);
            if (personalBD != null)
            {
                personalBD.Nombre = personal.Nombre;
                personalBD.Apellidos = personal.Apellidos;
                personalBD.Telefono = personal.Telefono;
                personalBD.Correo = personal.Correo;
                personalBD.Cedula = personal.Cedula;
                personalBD.FechaNacimiento = personal.FechaNacimiento;
                personalBD.FechaEntrada = personal.FechaEntrada;
                personalBD.RolPersonalId = personal.RolPersonalId;

                _db.SaveChanges();

            }
        }

        public IEnumerable<SelectListItem> ObtenerTodosDropdownLista(string obj)
        {
            if (obj == "RolPersonal")
            {
                return _db.RolPersonal.Select(c => new SelectListItem
                {
                    Text = c.Nombre,
                    Value = c.Id.ToString()
                });
            }

            // Manejo predeterminado para otros casos
            return Enumerable.Empty<SelectListItem>();
        }
    }
}

