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
    public class InflableRepositorio : Repositorio<Inflable>, IInflableRepositorio
    {
        private readonly ApplicationDbContext _db;

        public InflableRepositorio(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Actualizar(Inflable inflable)
        {
            var inflableBD = _db.Inflables.FirstOrDefault(b => b.Id == inflable.Id);
            if (inflableBD != null)
            {
                if (inflable.ImageUrl != null)
                {
                    inflableBD.ImageUrl = inflable.ImageUrl;
                }
                inflableBD.Nombre = inflable.Nombre;
                inflableBD.Descripcion = inflable.Descripcion;
                inflableBD.Dimensiones = inflable.Dimensiones;
                inflableBD.Estado= inflable.Estado;
                inflableBD.Precio = inflable.Precio;
                inflableBD.PrecioHoraAdicional = inflable.PrecioHoraAdicional;
                inflableBD.CategoriaEdadId = inflable.CategoriaEdadId;
                inflableBD.CategoriaTamannoId = inflable.CategoriaTamannoId;
                inflableBD.PadreId = inflable.PadreId;

                _db.SaveChanges();

            }
        }

        public IEnumerable<SelectListItem> ObtenerTodosDropdownLista(string obj)
        {
            if(obj == "CategoriaTamanno")
            {
                return _db.CategoriaTammano.Select(r => new SelectListItem
                {
                    Text = r.Nombre,
                    Value = r.Id.ToString()
                });
            }
            if (obj == "CategoriaEdad")
            {
                return _db.CategoriasEdades.Select(r => new SelectListItem
                {
                    Text = r.Nombre,
                    Value = r.Id.ToString()
                });
            }
            return null;
        }
    }
}

