using Microsoft.AspNetCore.Mvc;
using SalticosAdmin.AccesoDeDatos.Repositorio;
using SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio;
using SalticosAdmin.Modelos;
using SalticosAdmin.Utilidades;

namespace SalticosAdmin.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ClienteController : Controller
    {

        private readonly IUnidadTrabajo _unidadTrabajo;
        public ClienteController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo = unidadTrabajo;
        }
        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> Upsert(int? id)
        {
            Cliente cliente = new Cliente();


            if (id == null)
            {
                // Crear una nueva herramienta
                return View(cliente);
            }

            // Actualizamos
            cliente = await _unidadTrabajo.Cliente.Obtener(id.GetValueOrDefault());
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                if (cliente.Id == 0)
                {
                    await _unidadTrabajo.Cliente.Agregar(cliente);
                    TempData[DS.Exitosa] = "Cliente creado Exitosamente";
                }
                else
                {
                    _unidadTrabajo.Cliente.Actualizar(cliente);
                    TempData[DS.Exitosa] = "Cliente actualizado Exitosamente";
                }
                await _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));
            }
            TempData[DS.Error] = "Error al grabar cliente";
            return View(cliente);
        }


        #region API
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var todos = await _unidadTrabajo.Cliente.ObtenerTodos();
            return Json(new { data = todos });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var clienteBd = await _unidadTrabajo.Cliente.Obtener(id);
            if (clienteBd == null)
            {
                return Json(new { success = false, message = "Error al borrar Cliente" });
            }
            _unidadTrabajo.Cliente.Remover(clienteBd);
            await _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Cliente borrada exitosamente" });
        }

        [ActionName("ValidarNombre")]
        public async Task<IActionResult> ValidarNombre(string nombre, int id = 0)
        {
            bool valor = false;
            var lista = await _unidadTrabajo.Herramienta.ObtenerTodos();
            if (id == 0)
            {
                valor = lista.Any(b => b.Nombre.ToLower().Trim() == nombre.ToLower().Trim());
            }
            else
            {
                valor = lista.Any(b => b.Nombre.ToLower().Trim() == nombre.ToLower().Trim() && b.Id != id);
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
