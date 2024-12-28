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
    public class EventoRepositorio : Repositorio<Evento>, IEventoRepositorio
    {
        private readonly ApplicationDbContext _db;

        public EventoRepositorio(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Actualizar(Evento evento)
        {
            var eventoBD = _db.Eventos.FirstOrDefault(b => b.Id == evento.Id);
            if (eventoBD != null)
            {
                eventoBD.Fecha = evento.Fecha;
                eventoBD.HoraInicio = evento.HoraInicio;
                eventoBD.HoraFinal = evento.HoraFinal;
                eventoBD.Direccion = evento.Direccion;
                eventoBD.Provincia = evento.Provincia;
                eventoBD.ClienteId = evento.ClienteId;

                _db.SaveChanges();

            }
        }

        public IEnumerable<SelectListItem> ObtenerTodosDropdownLista(string obj)
        {
            if(obj == "Cliente")
            {
                return _db.Clientes.Select(r => new SelectListItem
                {
                    Text = $"{r.Nombre} {r.Apellidos} - {r.Cedula}",
                    Value = r.Id.ToString()
                });
            }
            return null;
        }

    }
}

