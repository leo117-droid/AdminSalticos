using Microsoft.AspNetCore.Mvc;
using SalticosAdmin.AccesoDeDatos.Repositorio;
using SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio;
using SalticosAdmin.Modelos;
using SalticosAdmin.Utilidades;

namespace SalticosAdmin.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SeguroController : Controller
    {

        private readonly IUnidadTrabajo _unidadTrabajo;
        public SeguroController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo = unidadTrabajo;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            Seguro seguro = new Seguro();

            if (id == null)
            {
                return View(seguro);
            }

            // Actualizamos
            seguro = await _unidadTrabajo.Seguros.Obtener(id.GetValueOrDefault());
            if (seguro == null)
            {
                return NotFound();
            }
            return View(seguro);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Seguro seguro)
        {
            if (ModelState.IsValid)
            {

                // Validar que la fecha de inicio no sea posterior a la fecha de vencimiento
                if (seguro.FechaInicio > seguro.FechaVencimiento)
                {
                    TempData[DS.Error] = "La fecha de inicio no puede ser posterior a la fecha de vencimiento.";
                    return View(seguro);
                }

                var usuarioNombre = User.Identity.Name;

                if (seguro.Id == 0)
                {

                    await _unidadTrabajo.Seguros.Agregar(seguro);
                    TempData[DS.Exitosa] = "Seguro creado Exitosamente";

                    await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se agregó el seguro '{seguro.NumeroPoliza}'", usuarioNombre);

                }
                else
                {

                    _unidadTrabajo.Seguros.Actualizar(seguro);
                    TempData[DS.Exitosa] = "Capacitacion actualizada Exitosamente";

                    await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se actualizó el seguro '{seguro.NumeroPoliza}'", usuarioNombre);

                }
                await _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));
            }
            TempData[DS.Error] = "Error al grabar seguro";
            return View(seguro);
        }


        #region API
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var todos = await _unidadTrabajo.Seguros.ObtenerTodos();
            return Json(new { data = todos });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var usuarioNombre = User.Identity.Name;

            var seguroBd = await _unidadTrabajo.Seguros.Obtener(id);
            if (seguroBd == null)
            {
                return Json(new { success = false, message = "Error al borrar seguro" });
            }
            _unidadTrabajo.Seguros.Remover(seguroBd);
            await _unidadTrabajo.Guardar();

            await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se eliminó el seguro '{seguroBd.NumeroPoliza}'", usuarioNombre);

            return Json(new { success = true, message = "Seguro borrado exitosamente" });
        }

        [ActionName("ValidarPoliza")]
        public async Task<IActionResult> ValidarPoliza(int poliza, int id = 0)
        {
            bool valor = false;
            var lista = await _unidadTrabajo.Seguros.ObtenerTodos();
            if (id == 0)
            {
                valor = lista.Any(b => b.NumeroPoliza == poliza);
            }
            else
            {
                valor = lista.Any(b => b.NumeroPoliza == poliza && b.Id != id);
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
