using Microsoft.AspNetCore.Mvc;
using SalticosAdmin.AccesoDeDatos.Repositorio;
using SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio;
using SalticosAdmin.Modelos;
using SalticosAdmin.Utilidades;

namespace SalticosAdmin.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProveedorController : Controller
    {

        private readonly IUnidadTrabajo _unidadTrabajo;
        public ProveedorController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo = unidadTrabajo;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            Proveedor proveedor = new Proveedor();

            if (id == null)
            {
                return View(proveedor);
            }

            // Actualizamos
            proveedor = await _unidadTrabajo.Proveedor.Obtener(id.GetValueOrDefault());
            if (proveedor == null)
            {
                return NotFound();
            }
            return View(proveedor);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Proveedor proveedor)
        {
            if (ModelState.IsValid)
            {

                var usuarioNombre = User.Identity.Name;

                if (proveedor.Id == 0)
                {
                    await _unidadTrabajo.Proveedor.Agregar(proveedor);
                    TempData[DS.Exitosa] = "Proveedor creado exitosamente";

                    await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se agregó el proveedor '{proveedor.NombreEmpresa}'", usuarioNombre);

                }
                else
                {
                    _unidadTrabajo.Proveedor.Actualizar(proveedor);
                    TempData[DS.Exitosa] = "Proveedor actualizado exitosamente";

                    await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se actualizó el proveedor '{proveedor.NombreEmpresa}'", usuarioNombre);

                }
                await _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));
            }
            TempData[DS.Error] = "Error al grabar proveedor";
            return View(proveedor);
        }


        #region API
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var todos = await _unidadTrabajo.Proveedor.ObtenerTodos();
            return Json(new { data = todos });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var usuarioNombre = User.Identity.Name;

            var proveedorBd = await _unidadTrabajo.Proveedor.Obtener(id);
            if (proveedorBd == null)
            {
                return Json(new { success = false, message = "Error al borrar proveedor" });
            }
            _unidadTrabajo.Proveedor.Remover(proveedorBd);
            await _unidadTrabajo.Guardar();

            await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se eliminó la capacitación '{proveedorBd.NombreEmpresa}'", usuarioNombre);

            return Json(new { success = true, message = "Proveedor borrados exitosamente" });
        }

        #endregion
    }
}
