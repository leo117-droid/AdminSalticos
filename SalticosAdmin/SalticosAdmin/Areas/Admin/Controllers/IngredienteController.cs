using Microsoft.AspNetCore.Mvc;
using SalticosAdmin.AccesoDeDatos.Repositorio;
using SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio;
using SalticosAdmin.Modelos;
using SalticosAdmin.Utilidades;

namespace SalticosAdmin.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class IngredienteController : Controller
    {

        private readonly IUnidadTrabajo _unidadTrabajo;
        public IngredienteController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo = unidadTrabajo;
        }
        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> Upsert(int? id)
        {
            Ingrediente ingrediente = new Ingrediente();


            if (id == null)
            {
                // Crear una nuevo ingrediente
                return View(ingrediente);
            }

            // Actualizamos
            ingrediente = await _unidadTrabajo.Ingrediente.Obtener(id.GetValueOrDefault());
            if (ingrediente == null)
            {
                return NotFound();
            }
            return View(ingrediente);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Ingrediente ingrediente)
        {
            if (ModelState.IsValid)
            {
                if (ingrediente.Id == 0)
                {
                    await _unidadTrabajo.Ingrediente.Agregar(ingrediente);
                    TempData[DS.Exitosa] = "Ingrediente creado Exitosamente";
                }
                else
                {
                    _unidadTrabajo.Ingrediente.Actualizar(ingrediente);
                    TempData[DS.Exitosa] = "Ingrediente actualizado Exitosamente";
                }
                await _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));
            }
            TempData[DS.Error] = "Error al grabar ingrediente";
            return View(ingrediente);
        }


        #region API
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var todos = await _unidadTrabajo.Ingrediente.ObtenerTodos();
            return Json(new { data = todos });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var ingredienteBd = await _unidadTrabajo.Ingrediente.Obtener(id);
            if (ingredienteBd == null)
            {
                return Json(new { success = false, message = "Error al borrar Ingrediente" });
            }
            _unidadTrabajo.Ingrediente.Remover(ingredienteBd);
            await _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Ingrediente borrada exitosamente" });
        }

        [ActionName("ValidarNombre")]
        public async Task<IActionResult> ValidarNombre(string nombre, int id = 0)
        {
            bool valor = false;
            var lista = await _unidadTrabajo.Ingrediente.ObtenerTodos();
            if (id == 0)
            {
                if(nombre != null)
                {
                    valor = lista.Any(b => b.Nombre.ToLower().Trim() == nombre.ToLower().Trim());
                }
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
