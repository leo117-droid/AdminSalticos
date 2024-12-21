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
                    string upload = webRootPath + DS.ImagenRutaServicioAdicional;
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);
                    using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }
                    servicioAdicional.ImageUrl = fileName + extension;


                    await _unidadTrabajo.ServicioAdicional.Agregar(servicioAdicional);
                    TempData[DS.Exitosa] = "Servicio Adicional creado Exitosamente";

                }
                else
                {
                    var objServicioAdicional = await _unidadTrabajo.ServicioAdicional.ObtenerPrimero(p => p.Id == servicioAdicional.Id, isTracking: false);
                    if (files.Count > 0) // Si se carga una nueva imagen
                    {
                        string upload = webRootPath + DS.ImagenRutaServicioAdicional;
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);

                        // Borrar la imagen anterior
                        var anteriorFile = Path.Combine(upload, objServicioAdicional.ImageUrl);
                        if (System.IO.File.Exists(anteriorFile))
                        {
                            System.IO.File.Delete(anteriorFile);
                        }
                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }
                        servicioAdicional.ImageUrl = fileName + extension;
                    } // Caso contrario no se carga una nueva imagen
                    else
                    {
                        servicioAdicional.ImageUrl = objServicioAdicional.ImageUrl;
                    }

                    _unidadTrabajo.ServicioAdicional.Actualizar(servicioAdicional);
                    TempData[DS.Exitosa] = "Servicio adicional actualizado Exitosamente";

                }
                await _unidadTrabajo.Guardar();
                return View("Index");
            }
            return View(servicioAdicional);
        }



        #region API
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var todos = await _unidadTrabajo.ServicioAdicional.ObtenerTodos();
            return Json(new { data = todos });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var servicioAdicionalBd = await _unidadTrabajo.ServicioAdicional.Obtener(id);
            if (servicioAdicionalBd == null)
            {
                return Json(new { success = false, message = "Error al borrar Servicio Adicional" });
            }

            //Remover imagen
            string upload = _webHostEnvironment.WebRootPath + DS.ImagenRutaServicioAdicional;
            var anteriorFile = Path.Combine(upload, servicioAdicionalBd.ImageUrl);
            if (System.IO.File.Exists(anteriorFile))
            {
                System.IO.File.Delete(anteriorFile);
            }

            _unidadTrabajo.ServicioAdicional.Remover(servicioAdicionalBd);
            await _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Servicio Adicional borrado exitosamente" });
        }

        [ActionName("ValidarNombre")]
        public async Task<IActionResult> ValidarNombre(string nombre, int id = 0)
        {
            bool valor = false;
            var lista = await _unidadTrabajo.ServicioAdicional.ObtenerTodos();
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
