using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalticosAdmin.AccesoDeDatos.Data;
using SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio;
using SalticosAdmin.Modelos;
using SalticosAdmin.Utilidades;

namespace SalticosAdmin.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsuarioController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;
        private readonly ApplicationDbContext _db;

        public UsuarioController(IUnidadTrabajo unidadTrabajo, ApplicationDbContext db)
        {
            _unidadTrabajo = unidadTrabajo;
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }




        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Usuario usuario = await _unidadTrabajo.Usuario.ObtenerPorIdAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                var usuarioNombre = User.Identity.Name;

                _unidadTrabajo.Usuario.Actualizar(usuario);
                TempData[DS.Exitosa] = "Usuario actualizado exitosamente";
                await _unidadTrabajo.Guardar();

                await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se actualizó el usuario '{usuario.Nombre}' '{usuario.Apellido}' ", usuarioNombre);

                return RedirectToAction(nameof(Index));
            }

            TempData[DS.Error] = "Error al grabar cliente";
            return View(usuario);
        }






        #region API

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var usuarioLista = await _unidadTrabajo.Usuario.ObtenerTodos();
            return Json(new { data = usuarioLista });
        }



        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var usuarioBD = await _unidadTrabajo.Usuario.ObtenerPorIdAsync(id);

            var usuarioNombre = User.Identity.Name;

            if (usuarioBD == null)
            {
                return Json(new { success = false, message = "Error al borrar Usuario" });
            }
            _unidadTrabajo.Usuario.Remover(usuarioBD);
            await _unidadTrabajo.Guardar();

            await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se eliminó el usuario '{usuarioBD.Nombre}' '{usuarioBD.Apellido}'", usuarioNombre);

            return Json(new { success = true, message = "Usuario borrado exitosamente" });
        }




        #endregion

    }
}
