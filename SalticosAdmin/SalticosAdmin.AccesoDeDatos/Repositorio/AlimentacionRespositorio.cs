using Microsoft.AspNetCore.Mvc.Rendering;
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
    public class AlimentacionRepositorio : Repositorio<Alimentacion>, IAlimentacionRepositorio
    {
        private readonly ApplicationDbContext _db;

        public AlimentacionRepositorio(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Actualizar(Alimentacion alimentacion)
        {
            var alimentacionlBD = _db.Alimentacion.FirstOrDefault(b => b.Id == alimentacion.Id);
            if (alimentacionlBD != null)
            {
                if (alimentacionlBD.ImageUrl != null)
                {
                    alimentacionlBD.ImageUrl = alimentacion.ImageUrl;
                }
                alimentacionlBD.Nombre = alimentacion.Nombre;
                alimentacionlBD.Descripcion = alimentacion.Descripcion;
                alimentacionlBD.Precio = alimentacion.Precio;

                _db.SaveChanges();

            }
        }
    }
}

