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
    public class MobiliarioRepositorio : Repositorio<Mobiliario>, IMobiliarioRepositorio
    {
        private readonly ApplicationDbContext _db;

        public MobiliarioRepositorio(ApplicationDbContext db) : base(db)
        {
            _db = db;   
        }
        public void Actualizar(Mobiliario mobiliario)
        {
            var mobiliarioBD = _db.Mobilarios.FirstOrDefault(b => b.Id == mobiliario.Id);
            if (mobiliarioBD != null)
            {
                if (mobiliario.ImageUrl != null)
                {
                    mobiliarioBD.ImageUrl = mobiliario.ImageUrl;
                }
                mobiliarioBD.Nombre = mobiliario.Nombre;
                mobiliarioBD.Descripcion = mobiliario.Descripcion;
                mobiliarioBD.Dimensiones = mobiliario.Dimensiones;
                mobiliarioBD.Descripcion = mobiliario.Descripcion;
                mobiliarioBD.Inventario = mobiliario.Inventario;
                mobiliarioBD.Precio = mobiliario.Precio;

                _db.SaveChanges();

            }
        }

        public async Task<List<Mobiliario>> ObtenerMobiliariosSolapados(DateTime fechaEvento, TimeSpan horaInicio, TimeSpan horaFin)
        {
            return await _db.Mobilarios
                .Where(m => _db.EventoMobiliario
                    .Any(em => em.IdMobiliario == m.Id &&
                               em.Evento.Fecha == fechaEvento &&
                               em.Evento.HoraInicio < horaFin &&
                               em.Evento.HoraFinal > horaInicio))
                .ToListAsync();
        }

    }
}
