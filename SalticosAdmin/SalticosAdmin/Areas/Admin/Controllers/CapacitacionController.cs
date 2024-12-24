using Microsoft.AspNetCore.Mvc;
using SalticosAdmin.AccesoDeDatos.Repositorio;
using SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio;
using SalticosAdmin.Modelos;
using SalticosAdmin.Utilidades;

namespace SalticosAdmin.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CapacitacionController : Controller
    {

        private readonly IUnidadTrabajo _unidadTrabajo;
        public CapacitacionController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo = unidadTrabajo;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            Capacitacion capacitacion = new Capacitacion();

            if (id == null)
            {
                // Crear una nueva capacitacion
                return View(capacitacion);
            }

            // Actualizamos
            capacitacion = await _unidadTrabajo.Capacitacion.Obtener(id.GetValueOrDefault());
            if (capacitacion == null)
            {
                return NotFound();
            }
            return View(capacitacion);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Capacitacion capacitacion)
        {
            if (ModelState.IsValid)
            {

                //var usuarioNombre = User.Identity.Name;
                var usuarioNombre = "usuarioPrueba";

                if (capacitacion.Id == 0)
                {
                    await _unidadTrabajo.Capacitacion.Agregar(capacitacion);
                    TempData[DS.Exitosa] = "Capacitacion creada Exitosamente";

                    await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se insertó la capacitación '{capacitacion.Tema}'", usuarioNombre);

                }
                else
                {
                    _unidadTrabajo.Capacitacion.Actualizar(capacitacion);
                    TempData[DS.Exitosa] = "Capacitacion actualizada Exitosamente";

                    await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se actualizó la capacitación '{capacitacion.Tema}'", usuarioNombre);

                }
                await _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));
            }
            TempData[DS.Error] = "Error al grabar capacitacion";
            return View(capacitacion);
        }


        #region API
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var todos = await _unidadTrabajo.Capacitacion.ObtenerTodos();
            return Json(new { data = todos });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            //var usuarioNombre = User.Identity.Name;
            var usuarioNombre = "usuarioPrueba";

            var capacitacionBd = await _unidadTrabajo.Capacitacion.Obtener(id);
            if (capacitacionBd == null)
            {
                return Json(new { success = false, message = "Error al borrar capacitacion" });
            }
            _unidadTrabajo.Capacitacion.Remover(capacitacionBd);
            await _unidadTrabajo.Guardar();

            await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se eliminó la capacitación '{capacitacionBd.Tema}'", usuarioNombre);

            return Json(new { success = true, message = "Capacitacion borrada exitosamente" });
        }

        #endregion
    }
}
