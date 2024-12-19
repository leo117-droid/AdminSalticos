using Microsoft.AspNetCore.Mvc;
using SalticosAdmin.AccesoDeDatos.Repositorio;
using SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio;
using SalticosAdmin.Modelos;
using SalticosAdmin.Utilidades;

namespace SalticosAdmin.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoriaTamannoController : Controller
    {

        private readonly IUnidadTrabajo _unidadTrabajo;
        public CategoriaTamannoController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo = unidadTrabajo;
        }
        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> Upsert(int? id)
        {
            CategoriaTamanno categoriaTamanno = new CategoriaTamanno();


            if (id == null)
            {
                // Crear una nueva categoria por tamanno
                return View(categoriaTamanno);
            }

            // Actualizamos
            categoriaTamanno = await _unidadTrabajo.CategoriaTamanno.Obtener(id.GetValueOrDefault());
            if (categoriaTamanno == null)
            {
                return NotFound();
            }
            return View(categoriaTamanno);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(CategoriaTamanno categoriaTamanno)
        {
            if (ModelState.IsValid)
            {
                if (categoriaTamanno.Id == 0)
                {
                    await _unidadTrabajo.CategoriaTamanno.Agregar(categoriaTamanno);
                    TempData[DS.Exitosa] = "Categoria por tamanno creado Exitosamente";
                }
                else
                {
                    _unidadTrabajo.CategoriaTamanno.Actualizar(categoriaTamanno);
                    TempData[DS.Exitosa] = "Categoria por tamanno actualizado Exitosamente";
                }
                await _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));
            }
            TempData[DS.Error] = "Error al grabar categoria por tamanno";
            return View(categoriaTamanno);
        }


        #region API
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var todos = await _unidadTrabajo.CategoriaTamanno.ObtenerTodos();
            return Json(new { data = todos });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var categoriaTamannoBd = await _unidadTrabajo.CategoriaTamanno.Obtener(id);
            if (categoriaTamannoBd == null)
            {
                return Json(new { success = false, message = "Error al borrar Categoría por tamanno" });
            }
            _unidadTrabajo.CategoriaTamanno.Remover(categoriaTamannoBd);
            await _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Categoría por tamanno borrada exitosamente" });
        }

        [ActionName("ValidarNombre")]
        public async Task<IActionResult> ValidarNombre(string nombre, int id = 0)
        {
            bool valor = false;
            var lista = await _unidadTrabajo.CategoriaTamanno.ObtenerTodos();
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
