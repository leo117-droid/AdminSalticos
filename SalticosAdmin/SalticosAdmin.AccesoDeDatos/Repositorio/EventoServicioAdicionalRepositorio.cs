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
    public class EventoServicioAdicionalRepositorio : Repositorio<EventoServicioAdicional>, IEventoServicioAdicionalRepositorio
    {
        private readonly ApplicationDbContext _db;

        public EventoServicioAdicionalRepositorio(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Actualizar(EventoServicioAdicional eventoServicioAdicional)
        {
            var eventoServicioAdicionalBD = _db.EventoServicioAdicional.FirstOrDefault(b => b.Id == eventoServicioAdicional.Id);
            if (eventoServicioAdicionalBD != null)
            {
                eventoServicioAdicionalBD.IdServicioAdicional = eventoServicioAdicional.IdServicioAdicional;
                eventoServicioAdicionalBD.IdEvento = eventoServicioAdicional.IdEvento;
                eventoServicioAdicionalBD.Cantidad = eventoServicioAdicional.Cantidad;
                _db.SaveChanges();

            }
        }

        public IEnumerable<SelectListItem> ObtenerServicioAdicional(string obj, int? idEvento)
        {
            if (obj.Equals("ServicioAdicional")){ 
                var servicio = _db.ServiciosAdicionales
                                .Where(t => !_db.EventoServicioAdicional
                                            .Any(cp => cp.IdServicioAdicional == t.Id && cp.IdEvento == idEvento))
                                .Select(c => new SelectListItem
                                {
                                    Text = $"{c.Nombre}",
                                    Value = c.Id.ToString()
                                })
                                .ToList();
                return servicio;
            }
            return null;
        }

    }
}

