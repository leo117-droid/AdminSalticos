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
    public class ProveedorRepositorio : Repositorio<Proveedor>, IProveedorRepositorio
    {
        private readonly ApplicationDbContext _db;

        public ProveedorRepositorio(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Actualizar(Proveedor proveedor)
        {
            var proveedorDB = _db.Proveedor.FirstOrDefault(p => p.Id == proveedor.Id);
            if (proveedorDB != null)
            {
                proveedorDB.NombreEmpresa = proveedor.NombreEmpresa;
                proveedorDB.Contacto = proveedor.Contacto;
                proveedorDB.Telefono = proveedor.Telefono;
                proveedorDB.Correo = proveedor.Correo;
                proveedorDB.Direccion = proveedor.Direccion;
                proveedorDB.Descripcion = proveedor.Descripcion;
                proveedorDB.TipoProveedor = proveedor.TipoProveedor;

                _db.SaveChanges();
            }
        }
    }
}

