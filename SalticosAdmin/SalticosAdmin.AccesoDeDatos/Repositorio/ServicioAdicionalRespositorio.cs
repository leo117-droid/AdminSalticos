using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SalticosAdmin.AccesoDeDatos.Data;
using SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio;
using SalticosAdmin.Modelos;
using SalticosAdmin.Modelos.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SalticosAdmin.AccesoDeDatos.Repositorio
{
    public class ServicioAdicionalRepositorio : Repositorio<ServicioAdicional>, IServicioAdicionalRepositorio
    {
        private readonly ApplicationDbContext _db;

        public ServicioAdicionalRepositorio(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Actualizar(ServicioAdicional servicioAdicional)
        {
            var servicioAdicionalBD = _db.ServiciosAdicionales.FirstOrDefault(b => b.Id == servicioAdicional.Id);
            if (servicioAdicionalBD != null)
            {
                if (servicioAdicionalBD.ImageUrl != null)
                {
                    servicioAdicionalBD.ImageUrl = servicioAdicional.ImageUrl;
                }
                servicioAdicionalBD.Nombre = servicioAdicional.Nombre;
                servicioAdicionalBD.Descripcion = servicioAdicional.Descripcion;
                servicioAdicionalBD.Inventario = servicioAdicional.Inventario;
                servicioAdicionalBD.Precio = servicioAdicional.Precio;

                _db.SaveChanges();

            }
        }

        public async Task<List<ServicioAdicional>> ObtenerServiciosAdicionalesSolapados(DateTime fechaEvento, TimeSpan horaInicio, TimeSpan horaFin)
        {
            return await _db.ServiciosAdicionales
                .Where(m => _db.EventoServicioAdicional
                    .Any(em => em.IdServicioAdicional == m.Id &&
                               em.Evento.Fecha == fechaEvento &&
                               em.Evento.HoraInicio < horaFin &&
                               em.Evento.HoraFinal > horaInicio))
                .ToListAsync();
        }
    }
}

