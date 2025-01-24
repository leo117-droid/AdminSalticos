using Microsoft.AspNetCore.Mvc;
using SalticosAdmin.AccesoDeDatos.Repositorio;
using SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio;
using SalticosAdmin.Modelos;
using SalticosAdmin.Utilidades;

namespace SalticosAdmin.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RolPersonalController : Controller
    {

        private readonly IUnidadTrabajo _unidadTrabajo;
        public RolPersonalController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo = unidadTrabajo;
        }
        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> Upsert(int? id)
        {
            RolPersonal rolPersonal = new RolPersonal();


            if (id == null)
            {
               
                return View(rolPersonal);
            }

            // Actualizamos
            rolPersonal = await _unidadTrabajo.RolPersonal.Obtener(id.GetValueOrDefault());
            if (rolPersonal == null)
            {
                return NotFound();
            }
            return View(rolPersonal);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(RolPersonal rolPersonal)
        {
            if (ModelState.IsValid)
            {
                var usuarioNombre = User.Identity.Name;

                if (rolPersonal.Id == 0)
                {
                    await _unidadTrabajo.RolPersonal.Agregar(rolPersonal);
                    TempData[DS.Exitosa] = "Rol de personal creado Exitosamente";

                    await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se agregó el rol de personal '{rolPersonal.Nombre}'", usuarioNombre);

                }
                else
                {
                    _unidadTrabajo.RolPersonal.Actualizar(rolPersonal);
                    TempData[DS.Exitosa] = "Rol de personal actualizado Exitosamente";

                    await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se actualizó el rol de personal '{rolPersonal.Nombre}'", usuarioNombre);

                }
                await _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));
            }
            TempData[DS.Error] = "Error al grabar rol de personal";
            return View(rolPersonal);
        }


        #region API
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var todos = await _unidadTrabajo.RolPersonal.ObtenerTodos();
            return Json(new { data = todos });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var rolPersonalBd = await _unidadTrabajo.RolPersonal.Obtener(id);

            var usuarioNombre = User.Identity.Name;

            if (rolPersonalBd == null)
            {
                return Json(new { success = false, message = "Error al borrar rol de personal" });
            }
            try
            {
                _unidadTrabajo.RolPersonal.Remover(rolPersonalBd);
                await _unidadTrabajo.Guardar();

                await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se eliminó el rol de personal '{rolPersonalBd.Nombre}'", usuarioNombre);

                return Json(new { success = true, message = "Rol de personal borrada exitosamente" });
            } catch (Exception ex) {
                return Json(new { success = false, message = "Rol ocupado por personal" });
            }
        }

        [ActionName("ValidarNombre")]
        public async Task<IActionResult> ValidarNombre(string nombre, int id = 0)
        {
            bool valor = false;
            var lista = await _unidadTrabajo.RolPersonal.ObtenerTodos();
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
