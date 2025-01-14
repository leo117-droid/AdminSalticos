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
    public class EventoPersonalRepositorio : Repositorio<EventoPersonal>, IEventoPersonalRepositorio
    {
        private readonly ApplicationDbContext _db;

        public EventoPersonalRepositorio(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Actualizar(EventoPersonal eventoPersonal)
        {
            var eventoPersonalBD = _db.EventoPersonal.FirstOrDefault(b => b.Id == eventoPersonal.Id);
            if (eventoPersonalBD != null)
            {
                eventoPersonalBD.IdPersonal = eventoPersonal.IdPersonal;
                eventoPersonalBD.IdEvento = eventoPersonal.IdEvento;
                _db.SaveChanges();

            }
        }

        public IEnumerable<SelectListItem> ObtenerPersonal(string obj, int? idEvento)
        {
            if (obj.Equals("Personal")){
                var eventoActual = _db.Eventos.FirstOrDefault(e => e.Id == idEvento);
                if (eventoActual == null)
                {
                    return null;
                }
                var personal = _db.Personal
                                .Where(t => !_db.EventoPersonal
                                            .Any(ep => ep.IdPersonal == t.Id && 
                                            (
                                                ep.IdEvento == idEvento ||
                                                _db.Eventos.Any(ev => ev.Id == ep.IdEvento &&
                                                             ev.Fecha == eventoActual.Fecha &&
                                                             ev.HoraInicio < eventoActual.HoraFinal &&
                                                             ev.HoraFinal > eventoActual.HoraInicio)
                                            )
                                            ))
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

