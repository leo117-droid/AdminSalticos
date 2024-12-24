using Microsoft.AspNetCore.Mvc;
using SalticosAdmin.AccesoDeDatos.Repositorio;
using SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio;
using SalticosAdmin.Modelos;
using SalticosAdmin.Utilidades;

namespace SalticosAdmin.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class VehiculoController : Controller
    {

        private readonly IUnidadTrabajo _unidadTrabajo;
        public VehiculoController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo = unidadTrabajo;
        }
        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> Upsert(int? id)
        {
            Vehiculo vehiculo = new Vehiculo();


            if (id == null)
            {
                // Crear una nueva herramienta
                return View(vehiculo);
            }

            // Actualizamos
            vehiculo = await _unidadTrabajo.Vehiculo.Obtener(id.GetValueOrDefault());
            if (vehiculo == null)
            {
                return NotFound();
            }
            return View(vehiculo);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Vehiculo vehiculo)
        {
            if (ModelState.IsValid)
            {
                //var usuarioNombre = User.Identity.Name;
                var usuarioNombre = "usuarioPrueba";

                if (vehiculo.Id == 0)
                {
                    await _unidadTrabajo.Vehiculo.Agregar(vehiculo);
                    TempData[DS.Exitosa] = "Vehiculo creado Exitosamente";

                    await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se insertó el vehiculo '{vehiculo.Placa}'", usuarioNombre);

                }
                else
                {
                    _unidadTrabajo.Vehiculo.Actualizar(vehiculo);
                    TempData[DS.Exitosa] = "Vehiculo actualizado Exitosamente";

                    await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se actualizó el vehiculo '{vehiculo.Placa}'", usuarioNombre);

                }
                await _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));
            }
            TempData[DS.Error] = "Error al grabar vehiculo";
            return View(vehiculo);
        }

        #region API
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var todos = await _unidadTrabajo.Vehiculo.ObtenerTodos();
            return Json(new { data = todos });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var vehiculoBd = await _unidadTrabajo.Vehiculo.Obtener(id);

            //var usuarioNombre = User.Identity.Name;
            var usuarioNombre = "usuarioPrueba";

            if (vehiculoBd == null)
            {
                return Json(new { success = false, message = "Error al borrar Vehiculo" });
            }
            _unidadTrabajo.Vehiculo.Remover(vehiculoBd);
            await _unidadTrabajo.Guardar();

            await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se eliminó el vehiculo '{vehiculoBd.Placa}'", usuarioNombre);

            return Json(new { success = true, message = "Vehiculo borrada exitosamente" });
        }

        [ActionName("ValidarPlaca")]
        public async Task<IActionResult> ValidarPlaca(string placa, int id = 0)
        {
            bool valor = false;
            var lista = await _unidadTrabajo.Vehiculo.ObtenerTodos();
            if (id == 0)
            {
                valor = lista.Any(b => b.Placa.ToLower().Trim() == placa.ToLower().Trim());
            }
            else
            {
                valor = lista.Any(b => b.Placa.ToLower().Trim() == placa.ToLower().Trim() && b.Id != id);
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
