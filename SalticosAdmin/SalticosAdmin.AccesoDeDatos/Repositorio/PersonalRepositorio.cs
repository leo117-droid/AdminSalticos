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
    public class CapacitacionRepositorio : Repositorio<Capacitacion>, ICapacitacionRepositorio
    {
        private readonly ApplicationDbContext _db;

        public CapacitacionRepositorio(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Actualizar(Capacitacion capacitacion)
        {
            var capacitacionBD = _db.Capacitaciones.FirstOrDefault(b => b.Id == capacitacion.Id);
            if (capacitacionBD != null)
            {
                capacitacionBD.Fecha = capacitacion.Fecha;
                capacitacionBD.Tema = capacitacion.Tema;
                capacitacionBD.Duracion = capacitacion.Duracion;

                _db.SaveChanges();

            }
        }

    }
}

