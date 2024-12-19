using Microsoft.AspNetCore.Mvc;
using SalticosAdmin.AccesoDeDatos.Repositorio;
using SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio;
using SalticosAdmin.Modelos;
using SalticosAdmin.Utilidades;

namespace SalticosAdmin.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PersonalController : Controller
    {

        private readonly IUnidadTrabajo _unidadTrabajo;
        public PersonalController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo = unidadTrabajo;
        }
        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> Upsert(int? id)
        {
            PersonalVM productoVM = new PersonalVM()
            {
                Personal = new Personal(),
                PersonalLista = _unidadTrabajo.Personal.ObtenerTodosDropdownLista("RolPersonal"),

            };

            if (id == null)
            {
                //crear un nuevo producto
                personalVM.Personal.Estado = true;

                return View(productoVM);
            }
            else
            {
                personalVM.Producto = await _unidadTrabajo.Personal.Obtener(id.GetValueOrDefault());
                if (productoVM.Producto == null)
                {
                    return NotFound();
                }
                return View(productoVM);
            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Personal personal)
        {
            if (ModelState.IsValid)
            {
                if (personal.Id == 0)
                {
                    await _unidadTrabajo.Personal.Agregar(personal);
                    TempData[DS.Exitosa] = "Personal creado Exitosamente";
                }
                else
                {
                    _unidadTrabajo.Personal.Actualizar(personal);
                    TempData[DS.Exitosa] = "Personal actualizado Exitosamente";
                }
                await _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));
            }
            TempData[DS.Error] = "Error al grabar personal";
            return View(personal);
        }


        #region API
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var todos = await _unidadTrabajo.Personal.ObtenerTodos();
            return Json(new { data = todos });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var personalBd = await _unidadTrabajo.Personal.Obtener(id);
            if (personalBd == null)
            {
                return Json(new { success = false, message = "Error al borrar Personal" });
            }
            _unidadTrabajo.Personal.Remover(personalBd);
            await _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Personal borrado exitosamente" });
        }


        [ActionName("ValidarCedula")]
        public async Task<IActionResult> ValidarCedula(string cedula, int id = 0)
        {
            bool valor = false;
            var lista = await _unidadTrabajo.Personal.ObtenerTodos();
            if (id == 0)
            {
                valor = lista.Any(b => b.Cedula.Trim() == cedula.Trim());
            }
            else
            {
                valor = lista.Any(b => b.Cedula.Trim() == cedula.Trim() && b.Id != id);
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
