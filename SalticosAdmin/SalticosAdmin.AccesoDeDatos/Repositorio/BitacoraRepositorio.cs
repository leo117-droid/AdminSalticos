using Microsoft.EntityFrameworkCore;
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
    public class BitacoraRepositorio : Repositorio<Bitacora>, IBitacoraRepositorio
    {
        private readonly ApplicationDbContext _db;

        public BitacoraRepositorio(ApplicationDbContext db) : base(db)
        {
            _db = db;   
        }

        public async Task RegistrarBitacora(string accion, string usuario)
        {
            var bitacora = new Bitacora
            {
                Fecha = DateTime.Now,
                Hora = DateTime.Now.TimeOfDay, 
                Accion = accion,
                Usuario = usuario

            };

            _db.Bitacora.Add(bitacora);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<Bitacora>> ObtenerPorFecha(DateTime fecha)
        {
            return await _db.Bitacora.Where(b => b.Fecha.Date == fecha.Date).ToListAsync();
        }

    }
}
