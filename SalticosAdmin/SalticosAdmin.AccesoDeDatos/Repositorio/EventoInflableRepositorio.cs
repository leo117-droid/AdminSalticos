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
    public class EventoInflableRepositorio : Repositorio<EventoInflable>, IEventoInflableRepositorio
    {
        private readonly ApplicationDbContext _db;

        public EventoInflableRepositorio(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Actualizar(EventoInflable eventoInflable)
        {
            var eventoInflableBD = _db.EventoInflable.FirstOrDefault(b => b.Id == eventoInflable.Id);
            if (eventoInflableBD != null)
            {
                eventoInflableBD.IdInflable = eventoInflable.IdInflable;
                eventoInflableBD.IdEvento = eventoInflable.IdEvento;
                _db.SaveChanges();

            }
        }

        public IEnumerable<SelectListItem> ObtenerInflable(string obj, int? idEvento)
        {
            if (obj.Equals("Inflable")) {

                var eventoActual = _db.Eventos.FirstOrDefault(e => e.Id == idEvento);
                if (eventoActual == null)
                {
                    return null; 
                }

                var inflablesDisponibles = _db.Inflables
                    .Where(t => !_db.EventoInflable
                        .Any(ei => ei.IdInflable == t.Id &&
                                   (
                                       ei.IdEvento == idEvento || 
                                       _db.Eventos.Any(ev => ev.Id == ei.IdEvento &&
                                                             ev.Fecha == eventoActual.Fecha && 
                                                             ev.HoraInicio < eventoActual.HoraFinal && 
                                                             ev.HoraFinal > eventoActual.HoraInicio)
                                   )
                        ))
                    .Select(c => new SelectListItem
                    {
                        Text = $"{c.Nombre}",
                        Value = c.Id.ToString()
                    })
                    .ToList();
                return inflablesDisponibles;
            }
            return null;
        }

    }
}

