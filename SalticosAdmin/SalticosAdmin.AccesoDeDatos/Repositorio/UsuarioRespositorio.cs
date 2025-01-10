using Microsoft.AspNetCore.Identity;
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
    public class UsuarioRepositorio : Repositorio<Usuario>, IUsuarioRepositorio
    {
        private readonly ApplicationDbContext _db;

        public UsuarioRepositorio(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }


        public void Actualizar(Usuario usuario)
        {
            var usuarioBD = _db.Usuario.FirstOrDefault(c => c.Id == usuario.Id);

            if (usuarioBD != null)
            {
                usuarioBD.Nombre = usuario.Nombre;
                usuarioBD.Apellido = usuario.Apellido;
                usuarioBD.Email = usuario.Email;

                _db.SaveChanges();
            }
        }

        public async Task<Usuario> ObtenerPorIdAsync(string id)
        {
            return await _db.Usuario.FindAsync(id);
        }

    }
}

