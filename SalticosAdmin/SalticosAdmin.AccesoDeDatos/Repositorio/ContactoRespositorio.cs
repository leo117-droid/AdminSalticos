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
    public class ContactoRepositorio : Repositorio<Contacto>, IContactoRepositorio
    {
        private readonly ApplicationDbContext _db;

        public ContactoRepositorio(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Actualizar(Contacto contacto)
        {
            var contactoBD = _db.Contactos.FirstOrDefault(b => b.Id == contacto.Id);
            if (contactoBD != null)
            {
                contactoBD.Nombre = contacto.Nombre;
                contactoBD.Apellido = contacto.Apellido;
                contactoBD.TipoServicio = contacto.TipoServicio;
                contactoBD.Direccion = contacto.Direccion;
                contactoBD.Telefono = contacto.Telefono;
                contactoBD.Correo = contacto.Correo;

                _db.SaveChanges();

            }
        }

    }
}
