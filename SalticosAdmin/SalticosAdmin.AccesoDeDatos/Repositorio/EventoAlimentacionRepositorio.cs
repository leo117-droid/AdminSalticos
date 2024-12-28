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
    public class EventoAlimentacionRepositorio : Repositorio<EventoAlimentacion>, IEventoAlimentacionRepositorio
    {
        private readonly ApplicationDbContext _db;

        public EventoAlimentacionRepositorio(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Actualizar(EventoAlimentacion eventoAlimentacion)
        {
            var eventoAlimentacionBD = _db.EventoAlimentacion.FirstOrDefault(b => b.Id == eventoAlimentacion.Id);
            if (eventoAlimentacionBD != null)
            {
                eventoAlimentacionBD.IdAlimentacion = eventoAlimentacion.IdAlimentacion;
                eventoAlimentacionBD.IdEvento = eventoAlimentacion.IdEvento;
                eventoAlimentacionBD.Cantidad = eventoAlimentacion.Cantidad;
                _db.SaveChanges();

            }
        }

        public IEnumerable<SelectListItem> ObtenerAlimentacion(string obj, int? idEvento)
        {
            if (obj.Equals("Alimentacion")){ // o Ingredientes 
                var alimentacion = _db.Alimentacion
                                .Where(t => !_db.EventoAlimentacion
                                            .Any(cp => cp.IdAlimentacion == t.Id && cp.IdEvento == idEvento))
                                .Select(c => new SelectListItem
                                {
                                    Text = $"{c.Nombre}",
                                    Value = c.Id.ToString()
                                })
                                .ToList();
                return alimentacion;
            }
            return null;
        }

    }
}

