﻿using Microsoft.AspNetCore.Mvc.Rendering;
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
    public class EventoVehiculoRepositorio : Repositorio<EventoVehiculo>, IEventoVehiculoRepositorio
    {
        private readonly ApplicationDbContext _db;

        public EventoVehiculoRepositorio(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Actualizar(EventoVehiculo eventoVehiculo)
        {
            var eventoVehiculoBD = _db.EventoVehiculo.FirstOrDefault(b => b.Id == eventoVehiculo.Id);
            if (eventoVehiculoBD != null)
            {
                eventoVehiculoBD.IdVehiculo = eventoVehiculo.IdVehiculo;
                eventoVehiculoBD.IdEvento = eventoVehiculo.IdEvento;
                _db.SaveChanges();

            }
        }

        public IEnumerable<SelectListItem> ObtenerVehiculo(string obj, int? idEvento)
        {
            if (obj.Equals("Vehiculo")){
                var vehiculosDisponibles = _db.Vehiculos
                                        .Where(t => !_db.EventoVehiculo
                                        .Any(evVeh => evVeh.IdVehiculo == t.Id &&
                                              (
                                                  evVeh.IdEvento == idEvento || 
                                                  _db.Eventos.Any(ev => ev.Id == evVeh.IdEvento &&
                                                                        ev.Id != idEvento && 
                                                                        _db.Eventos.Where(e => e.Id == idEvento)
                                                                                   .Any(evento => evento.Fecha == ev.Fecha && 
                                                                                                  evento.HoraInicio < ev.HoraFinal && 
                                                                                                  evento.HoraFinal > ev.HoraInicio)
                                                  )
                                              )
                                        ))
                                .Select(c => new SelectListItem
                                {
                                    Text = $"{c.Modelo} - {c.Placa}",
                                    Value = c.Id.ToString()
                                })
                                .ToList();
                return vehiculosDisponibles;
            }
            return null;
        }

    }
}

