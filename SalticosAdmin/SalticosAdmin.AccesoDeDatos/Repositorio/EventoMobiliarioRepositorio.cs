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
    public class EventoMobiliarioRepositorio : Repositorio<EventoMobiliario>, IEventoMobiliarioRepositorio
    {
        private readonly ApplicationDbContext _db;

        public EventoMobiliarioRepositorio(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Actualizar(EventoMobiliario eventoMobiliario)
        {
            var eventoMobiliarioBD = _db.EventoMobiliario.FirstOrDefault(b => b.Id == eventoMobiliario.Id);
            if (eventoMobiliarioBD != null)
            {
                eventoMobiliarioBD.IdMobiliario = eventoMobiliario.IdMobiliario;
                eventoMobiliarioBD.IdEvento = eventoMobiliario.IdEvento;
                eventoMobiliarioBD.Cantidad = eventoMobiliario.Cantidad;
                _db.SaveChanges();

            }
        }

        public IEnumerable<SelectListItem> ObtenerMobiliario(string obj, int? idEvento)
        {
            if (obj.Equals("Mobiliario")){ 
                var mobiliario = _db.Mobilarios
                                .Where(t => !_db.EventoMobiliario
                                            .Any(cp => cp.IdMobiliario == t.Id && cp.IdEvento == idEvento))
                                .Select(c => new SelectListItem
                                {
                                    Text = $"{c.Nombre}",
                                    Value = c.Id.ToString()
                                })
                                .ToList();
                return mobiliario;
            }
            return null;
        }

    }
}

