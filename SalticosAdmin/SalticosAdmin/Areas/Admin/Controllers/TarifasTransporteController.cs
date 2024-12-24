using Microsoft.AspNetCore.Mvc;
using SalticosAdmin.AccesoDeDatos.Repositorio;
using SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio;
using SalticosAdmin.Modelos;
using SalticosAdmin.Utilidades;

namespace SalticosAdmin.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TarifasTransporteController : Controller
    {

        private readonly IUnidadTrabajo _unidadTrabajo;
        public TarifasTransporteController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo = unidadTrabajo;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            TarifasTransporte tarifasTransporte = new TarifasTransporte();


            if (id == null)
            {
                return View(tarifasTransporte);
            }

            // Actualizamos
            tarifasTransporte = await _unidadTrabajo.TarifasTransporte.Obtener(id.GetValueOrDefault());
            if (tarifasTransporte == null)
            {
                return NotFound();
            }
            return View(tarifasTransporte);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(TarifasTransporte tarifasTransporte)
        {
            if (ModelState.IsValid)
            {
                //var usuarioNombre = User.Identity.Name;
                var usuarioNombre = "usuarioPrueba";

                if (tarifasTransporte.Id == 0)
                {
                    await _unidadTrabajo.TarifasTransporte.Agregar(tarifasTransporte);
                    TempData[DS.Exitosa] = "Tarifa de Transporte creada Exitosamente";

                    await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se insertó la tarifa de transporte '{tarifasTransporte.Provincia}'", usuarioNombre);

                }
                else
                {
                    _unidadTrabajo.TarifasTransporte.Actualizar(tarifasTransporte);
                    TempData[DS.Exitosa] = "Tarifa de Transporte actualizada Exitosamente";

                    await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se actualizo la tarifa de transporte '{tarifasTransporte.Provincia}'", usuarioNombre);

                }
                await _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));
            }
            TempData[DS.Error] = "Error al grabar Tarifa de Transporte";
            return View(tarifasTransporte);
        }


        #region API
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var todos = await _unidadTrabajo.TarifasTransporte.ObtenerTodos();
            return Json(new { data = todos });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var tarifaBd = await _unidadTrabajo.TarifasTransporte.Obtener(id);

            //var usuarioNombre = User.Identity.Name;
            var usuarioNombre = "usuarioPrueba";

            if (tarifaBd == null)
            {
                return Json(new { success = false, message = "Error al borrar Tarifa de Transporte" });
            }
            _unidadTrabajo.TarifasTransporte.Remover(tarifaBd);
            await _unidadTrabajo.Guardar();

            await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se eliminó la tarifa de transporte '{tarifaBd.Provincia}'", usuarioNombre);

            return Json(new { success = true, message = "Tarifa de Transporte borrada exitosamente" });
        }

        [ActionName("ValidarProvincia")]
        public async Task<IActionResult> ValidarProvincia(string provincia, int id = 0)
        {
            bool valor = false;
            var lista = await _unidadTrabajo.TarifasTransporte.ObtenerTodos();
            if (id == 0)
            {
                valor = lista.Any(b => b.Provincia.ToLower().Trim() == provincia.ToLower().Trim());
            }
            else
            {
                valor = lista.Any(b => b.Provincia.ToLower().Trim() == provincia.ToLower().Trim() && b.Id != id);
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
