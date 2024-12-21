using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SalticosAdmin.AccesoDeDatos.Repositorio;
using SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio;
using SalticosAdmin.Modelos;
using SalticosAdmin.Modelos.ViewModels;
using SalticosAdmin.Utilidades;

namespace SalticosAdmin.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ServicioAdicionalController : Controller
    {

        private readonly IUnidadTrabajo _unidadTrabajo;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ServicioAdicionalController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo = unidadTrabajo;
        }
        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> Upsert(int? id)
        {
            ServicioAdicional servicioAdicional = new ServicioAdicional();


            if (id == null)
            {
                // Crear una nuevo servicioAdicional
                return View(servicioAdicional);
            }

            // Actualizamos
            servicioAdicional = await _unidadTrabajo.ServicioAdicional.Obtener(id.GetValueOrDefault());
            if (servicioAdicional == null)
            {
                return NotFound();
            }
            return View(servicioAdicional);

        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(ServicioAdicional servicioAdicional)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnvironment.WebRootPath;
                if (servicioAdicional.Id == 0)
                {
                    string upload = webRootPath + DS.ImagenRutaInflable;
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);
                    using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }
                    servicioAdicional.ImageUrl = fileName + extension;


                    await _unidadTrabajo.Inflable.Agregar(inflableVM.Inflable);
                    TempData[DS.Exitosa] = "Inflable creado Exitosamente";

                }
                else
                {
                    var objInflable = await _unidadTrabajo.Inflable.ObtenerPrimero(p => p.Id == inflableVM.Inflable.Id, isTracking: false);
                    if (files.Count > 0) // Si se carga una nueva imagen
                    {
                        string upload = webRootPath + DS.ImagenRutaInflable;
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);

                        // Borrar la imagen anterior
                        var anteriorFile = Path.Combine(upload, objInflable.ImageUrl);
                        if (System.IO.File.Exists(anteriorFile))
                        {
                            System.IO.File.Delete(anteriorFile);
                        }
                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }
                        inflableVM.Inflable.ImageUrl = fileName + extension;
                    } // Caso contrario no se carga una nueva imagen
                    else
                    {
                        inflableVM.Inflable.ImageUrl = objInflable.ImageUrl;
                    }

                    _unidadTrabajo.Inflable.Actualizar(inflableVM.Inflable);
                    TempData[DS.Exitosa] = "Inflable actualizado Exitosamente";

                }
                await _unidadTrabajo.Guardar();
                return View("Index");
            }
            inflableVM.CategoriaEdadLista = _unidadTrabajo.Inflable.ObtenerTodosDropdownLista("CategoriaEdad");
            inflableVM.CategoriaTamannoLista = _unidadTrabajo.Inflable.ObtenerTodosDropdownLista("CategoriaTamanno");


            return View(inflableVM);
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
