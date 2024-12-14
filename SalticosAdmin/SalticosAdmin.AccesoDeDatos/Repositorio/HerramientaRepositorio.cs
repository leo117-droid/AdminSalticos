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
    public class HerramientaRepositorio : Repositorio<Herramienta>, IHerramientaRepositorio
    {
        private readonly ApplicationDbContext _db;

        public HerramientaRepositorio(ApplicationDbContext db) : base(db)
        {
            _db = db;   
        }
        public void Actualizar(Herramienta herramienta)
        {
            var herramientaBD = _db.Herramientas.FirstOrDefault(b => b.Id == herramienta.Id);
            if (herramientaBD != null)
            {
                herramientaBD.Nombre = herramienta.Nombre;
                herramientaBD.Descripcion = herramienta.Descripcion;
                herramientaBD.Cantidad = herramienta.Cantidad;
                _db.SaveChanges();

            }
        }

    }
}
