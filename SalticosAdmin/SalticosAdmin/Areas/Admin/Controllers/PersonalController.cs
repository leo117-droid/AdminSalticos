using Microsoft.AspNetCore.Mvc;
using SalticosAdmin.AccesoDeDatos.Repositorio;
using SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio;
using SalticosAdmin.Modelos;
using SalticosAdmin.Modelos.ViewModels;
using SalticosAdmin.Utilidades;

namespace SalticosAdmin.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PersonalController : Controller
    {

        private readonly IUnidadTrabajo _unidadTrabajo;
        private readonly IWebHostEnvironment _webHostEnviroment;


        public PersonalController(IUnidadTrabajo unidadTrabajo, IWebHostEnvironment webHostEnviroment)
        {
            _unidadTrabajo = unidadTrabajo;
            _webHostEnviroment = webHostEnviroment;
        }
        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> Upsert(int? id)
        {
            PersonalVM personalVM = new PersonalVM()
            {
                Personal = new Personal(),
                RolPersonalLista = _unidadTrabajo.Personal.ObtenerTodosDropdownLista("RolPersonal")
            };

            if(id == null)
            {
                //Crear nuevo personal
                return View(personalVM);
            }
            else
            {
                personalVM.Personal = await _unidadTrabajo.Personal.Obtener(id.GetValueOrDefault());
                if (personalVM.Personal == null)
                {
                    return NotFound();
                }
                return View(personalVM);
            }

        }


        


        #region API
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var todos = await _unidadTrabajo.Personal.ObtenerTodos(incluirPropiedades:"RolPersonal");
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
            var lista = await _unidadTrabajo.Cliente.ObtenerTodos();
            if (id == 0)
            {
                if(cedula != null)
                {
                    valor = lista.Any(b => b.Cedula.ToLower().Trim() == cedula.ToLower().Trim());
                }
            }
            else
            {
                valor = lista.Any(b => b.Cedula.ToLower().Trim() == cedula.ToLower().Trim() && b.Id != id);
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
