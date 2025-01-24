using Microsoft.AspNetCore.Mvc;
using SalticosAdmin.AccesoDeDatos.Repositorio;
using SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio;
using SalticosAdmin.Modelos;
using SalticosAdmin.Utilidades;
using System.Diagnostics.Contracts;

namespace SalticosAdmin.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ContactoController : Controller
    {

        private readonly IUnidadTrabajo _unidadTrabajo;
        public ContactoController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo = unidadTrabajo;
        }
        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> Upsert(int? id)
        {
            Contacto contacto = new Contacto();


            if (id == null)
            {
                // Crear una nuevo contacto 
                return View(contacto);
            }

            // Actualizamos
            contacto = await _unidadTrabajo.Contacto.Obtener(id.GetValueOrDefault());
            if (contacto == null)
            {
                return NotFound();
            }
            return View(contacto);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Contacto contacto)
        {
            if (ModelState.IsValid)
            {
                var usuarioNombre = User.Identity.Name;

                if (contacto.Id == 0)
                {
                    await _unidadTrabajo.Contacto.Agregar(contacto);
                    TempData[DS.Exitosa] = "Contacto creado Exitosamente";

                    await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se agregó el contacto '{contacto.Nombre}'", usuarioNombre);

                }
                else
                {
                    _unidadTrabajo.Contacto.Actualizar(contacto);
                    TempData[DS.Exitosa] = "Contacto actualizado Exitosamente";

                    await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se actualizó el contacto '{contacto.Nombre}'", usuarioNombre);

                }
                await _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));
            }
            TempData[DS.Error] = "Error al grabar contacto";
            return View(contacto);
        }


        #region API
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var todos = await _unidadTrabajo.Contacto.ObtenerTodos();
            return Json(new { data = todos });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var contactoBd = await _unidadTrabajo.Contacto.Obtener(id);

            var usuarioNombre = User.Identity.Name;
            
            if (contactoBd == null)
            {
                return Json(new { success = false, message = "Error al borrar Contacto" });
            }
            _unidadTrabajo.Contacto.Remover(contactoBd);
            await _unidadTrabajo.Guardar();

            await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se eliminó el contacto '{contactoBd.Nombre}'", usuarioNombre);

            return Json(new { success = true, message = "Contacto borrado exitosamente" });
        }

        [ActionName("ValidarNombre")]
        public async Task<IActionResult> ValidarNombre(string nombre, int id = 0)
        {
            bool valor = false;
            var lista = await _unidadTrabajo.Contacto.ObtenerTodos();
            if (id == 0)
            {
                if(nombre != null)
                {
                    valor = lista.Any(b => b.Nombre.ToLower().Trim() == nombre.ToLower().Trim());
                }
                
            }
            else
            {
                if (nombre != null)
                {
                    valor = lista.Any(b => b.Nombre.ToLower().Trim() == nombre.ToLower().Trim() && b.Id != id);
                }
            }
            if (valor)
            {
                return Json(new { data = true });
            }
            return Json(new { data = false });
        }

        #endregion
    }
}
