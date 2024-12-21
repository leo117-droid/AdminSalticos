using Microsoft.AspNetCore.Mvc;
using SalticosAdmin.AccesoDeDatos.Repositorio;
using SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio;
using SalticosAdmin.Modelos;
using SalticosAdmin.Modelos.ViewModels;
using SalticosAdmin.Utilidades;

namespace SalticosAdmin.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class InflableController : Controller
    {

        private readonly IUnidadTrabajo _unidadTrabajo;
        private readonly IWebHostEnvironment _webHostEnviroment;


        public InflableController(IUnidadTrabajo unidadTrabajo, IWebHostEnvironment webHostEnviroment)
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
            InflableVM inflableVM = new InflableVM()
            {
                Inflable = new Inflable(),
                CategoriaTamannoLista = _unidadTrabajo.Inflable.ObtenerTodosDropdownLista("CategoriaTamanno"),
                CategoriaEdadLista = _unidadTrabajo.Inflable.ObtenerTodosDropdownLista("CategoriaEdad")
            };

            if(id == null)
            {
                //Crear nuevo inflable
                return View(inflableVM);
            }
            else
            {
                inflableVM.Inflable = await _unidadTrabajo.Inflable.Obtener(id.GetValueOrDefault());
                if (inflableVM.Inflable == null)
                {
                    return NotFound();
                }
                return View(inflableVM);
            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(PersonalVM personalVM)
        {
            if (ModelState.IsValid)
            {
                if(personalVM.Personal.Id == 0)
                {
                    await _unidadTrabajo.Personal.Agregar(personalVM.Personal);
                    TempData[DS.Exitosa] = "Personal creado Exitosamente";

                }
                else
                {
                    _unidadTrabajo.Personal.Actualizar(personalVM.Personal);
                    TempData[DS.Exitosa] = "Personal actualizado Exitosamente";

                }
                await _unidadTrabajo.Guardar();
                return View("Index");
            }
            personalVM.RolPersonalLista = _unidadTrabajo.Personal.ObtenerTodosDropdownLista("RolPersonal");

            return View(personalVM);
        }


        #region API
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var todos = await _unidadTrabajo.Inflable.ObtenerTodos(incluirPropiedades: "CategoriaTamanno,CategoriasEdad");
            return Json(new { data = todos });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var inflableBd = await _unidadTrabajo.Inflable.Obtener(id);
            if (inflableBd == null)
            {
                return Json(new { success = false, message = "Error al borrar Inflable" });
            }
            _unidadTrabajo.Inflable.Remover(inflableBd);
            await _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Inflable borrado exitosamente" });
        }

        [ActionName("ValidarNombre")]
        public async Task<IActionResult> ValidarNombre(string nombre, int id = 0)
        {
            bool valor = false;
            var lista = await _unidadTrabajo.Inflable.ObtenerTodos();
            if (id == 0)
            {
                if (nombre != null)
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
