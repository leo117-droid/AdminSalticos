using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
                personalBD.PadreId = personal.PadreId;

                _db.SaveChanges();

            }
        }

        public IEnumerable<SelectListItem> ObtenerTodosDropdownLista(string obj)
        {
            if (obj == "RolPersonal")
            {
                return _db.RolPersonal.Select(r => new SelectListItem
                {
                    Text = r.Nombre,
                    Value = r.Id.ToString()
                });
            }
            return null;
        }

        public async Task<IEnumerable<Personal>> FiltrarPorRolPersonal(int rolPersonalId)
        {
            return await _db.Personal
                .Include(p => p.RolPersonal)
                .Where(p => p.RolPersonalId == rolPersonalId)
                .ToListAsync();
        }
    }
}

