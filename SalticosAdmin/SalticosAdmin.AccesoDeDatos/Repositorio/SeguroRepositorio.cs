using Microsoft.AspNetCore.Mvc.Rendering;
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
    public class SeguroRepositorio : Repositorio<Seguro>, ISeguroRepositorio
    {
        private readonly ApplicationDbContext _db;

        public SeguroRepositorio(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Actualizar(Seguro seguro)
        {
            var seguroBD = _db.Seguros.FirstOrDefault(b => b.Id == seguro.Id);
            if (seguroBD != null)
            {
                seguroBD.TipoSeguro = seguro.TipoSeguro;
                seguroBD.NombreAseguradora = seguro.NombreAseguradora;
                seguroBD.NumeroPoliza = seguro.NumeroPoliza;
                seguroBD.FechaInicio = seguro.FechaInicio;
                seguroBD.FechaVencimiento = seguro.FechaVencimiento;
                seguroBD.Estado = seguro.Estado;

                _db.SaveChanges();

            }
        }

    }
}

