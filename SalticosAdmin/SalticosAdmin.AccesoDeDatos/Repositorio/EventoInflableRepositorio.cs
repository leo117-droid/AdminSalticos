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
            if (obj.Equals("Inflable")){  
                var inflable = _db.Inflables
                                .Where(t => !_db.EventoInflable
                                            .Any(cp => cp.IdInflable == t.Id && cp.IdEvento == idEvento))
                                .Select(c => new SelectListItem
                                {
                                    Text = $"{c.Nombre}",
                                    Value = c.Id.ToString()
                                })
                                .ToList();
                return inflable;
            }
            return null;
        }

    }
}

