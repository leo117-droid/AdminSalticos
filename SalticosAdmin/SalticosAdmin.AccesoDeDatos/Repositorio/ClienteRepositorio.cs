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
    public class ClienteRepositorio : Repositorio<Cliente>, IClienteRepositorio
    {
        private readonly ApplicationDbContext _db;

        public ClienteRepositorio(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Actualizar(Cliente cliente)
        {
            var clienteBD = _db.Clientes.FirstOrDefault(b => b.Id == cliente.Id);
            if (clienteBD != null)
            {
                clienteBD.Nombre = cliente.Nombre;
                clienteBD.Apellidos = cliente.Apellidos;
                clienteBD.Cedula = cliente.Cedula;
                clienteBD.Telefono = cliente.Telefono;
                clienteBD.Correo = cliente.Correo;
                _db.SaveChanges();

            }
        }

    }
}
