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

        public async Task<IActionResult> Index()
        {
            PersonalVM personalVM = new PersonalVM()
            {
                Personal = new Personal(),
                RolPersonalLista = _unidadTrabajo.Personal.ObtenerTodosDropdownLista("RolPersonal"),
                PersonalLista = await _unidadTrabajo.Personal.ObtenerTodos(incluirPropiedades: "RolPersonal")
            };


            return View(personalVM);
        }


        public async Task<IActionResult> Upsert(int? id)
        {
            PersonalVM personalVM = new PersonalVM()
            {
                Personal = new Personal(),
                RolPersonalLista = _unidadTrabajo.Personal.ObtenerTodosDropdownLista("RolPersonal")
            };

            if (id == null)
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(PersonalVM personalVM)
        {

            if (ModelState.IsValid)
            {
                // Validación de edad mínima (18 años)
                var fechaMinima = DateTime.Now.AddYears(-18);
                if (personalVM.Personal.FechaNacimiento > fechaMinima)
                {
                    ModelState.AddModelError("Personal.FechaNacimiento", "El personal debe ser mayor de 18 años.");
                    personalVM.RolPersonalLista = _unidadTrabajo.Personal.ObtenerTodosDropdownLista("RolPersonal");
                    return View(personalVM);
                }  

                var usuarioNombre = User.Identity.Name;

                if (personalVM.Personal.Id == 0)
                {
                    await _unidadTrabajo.Personal.Agregar(personalVM.Personal);
                    TempData[DS.Exitosa] = "Personal creado Exitosamente";

                    await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se agregó el personal '{personalVM.Personal.Nombre}' '{personalVM.Personal.Apellidos}'", usuarioNombre);

                }
                else
                {
                    _unidadTrabajo.Personal.Actualizar(personalVM.Personal);
                    TempData[DS.Exitosa] = "Personal actualizado Exitosamente";

                    await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se actualizó el personal '{personalVM.Personal.Nombre}' '{personalVM.Personal.Apellidos}'", usuarioNombre);

                }
                await _unidadTrabajo.Guardar();
                return RedirectToAction("Index");
            }
            personalVM.RolPersonalLista = _unidadTrabajo.Personal.ObtenerTodosDropdownLista("RolPersonal");

            return View(personalVM);
        }

        #region API
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var todos = await _unidadTrabajo.Personal.ObtenerTodos(incluirPropiedades: "RolPersonal");
            return Json(new { data = todos });
        }

        [HttpGet]
        public async Task<IActionResult> ConsultarConFiltro(int? idRolPersonal)
        {
            var personalVM = new PersonalVM();

            personalVM.RolPersonalLista = _unidadTrabajo.Personal.ObtenerTodosDropdownLista("RolPersonal");

            if (idRolPersonal.HasValue)
            {
                personalVM.PersonalLista = await _unidadTrabajo.Personal.FiltrarPorRolPersonal(idRolPersonal.Value);
            }
            else
            {

                personalVM.PersonalLista = await _unidadTrabajo.Personal.ObtenerTodos(incluirPropiedades: "RolPersonal");
            }
            var resultados = personalVM.PersonalLista;
            return Json(new { data = resultados });
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var personalBd = await _unidadTrabajo.Personal.Obtener(id);

            var usuarioNombre = User.Identity.Name;

            if (personalBd == null)
            {
                return Json(new { success = false, message = "Error al borrar Personal" });
            }

            try
            {
                _unidadTrabajo.Personal.Remover(personalBd);
                await _unidadTrabajo.Guardar();

                await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se eliminó el personal '{personalBd.Nombre}' '{personalBd.Apellidos}'", usuarioNombre);

                return Json(new { success = true, message = "Personal borrado exitosamente" });
            } catch (Exception ex)
            {
                return Json(new { success = false, message = "Personal asignado a un evento o capacitacion" });
            }
        }

        [ActionName("ValidarCedula")]
        public async Task<IActionResult> ValidarCedula(string cedula, int id = 0)
        {
            bool valor = false;
            var lista = await _unidadTrabajo.Cliente.ObtenerTodos();
            if (id == 0)
            {
                if (cedula != null)
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
