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
    public class AlimentacionController : Controller
    {

        private readonly IUnidadTrabajo _unidadTrabajo;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AlimentacionController(IUnidadTrabajo unidadTrabajo, IWebHostEnvironment webHostEnvironment)
        {
            _unidadTrabajo = unidadTrabajo;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Intermedia()
        {
            return View();
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            Alimentacion alimentacion = new Alimentacion();


            if (id == null)
            {
                // Crear una nuevo alimentacion
                return View(alimentacion);
            }

            // Actualizamos
            alimentacion = await _unidadTrabajo.Alimentacion.Obtener(id.GetValueOrDefault());
            if (alimentacion == null)
            {
                return NotFound();
            }
            return View(alimentacion);

        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Alimentacion alimentacion)
        {
            if (ModelState.IsValid)
            {
                var usuarioNombre = User.Identity.Name;

                var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnvironment.WebRootPath;
                if (alimentacion.Id == 0)
                {
                    string upload = webRootPath + DS.ImagenRutaAlimentacion;
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);
                    using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }
                    alimentacion.ImageUrl = fileName + extension;


                    await _unidadTrabajo.Alimentacion.Agregar(alimentacion);
                    TempData[DS.Exitosa] = "Alimentacion creado Exitosamente";

                    await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se agregó la alimentacion '{alimentacion.Nombre}'", usuarioNombre);

                }
                else
                {
                    var objAlimentacion = await _unidadTrabajo.Alimentacion.ObtenerPrimero(p => p.Id == alimentacion.Id, isTracking: false);
                    if (files.Count > 0) // Si se carga una nueva imagen
                    {
                        string upload = webRootPath + DS.ImagenRutaAlimentacion;
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);

                        // Borrar la imagen anterior
                        var anteriorFile = Path.Combine(upload, objAlimentacion.ImageUrl);
                        if (System.IO.File.Exists(anteriorFile))
                        {
                            System.IO.File.Delete(anteriorFile);
                        }
                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }
                        alimentacion.ImageUrl = fileName + extension;
                    } // Caso contrario no se carga una nueva imagen
                    else
                    {
                        alimentacion.ImageUrl = objAlimentacion.ImageUrl;
                    }

                    _unidadTrabajo.Alimentacion.Actualizar(alimentacion);
                    TempData[DS.Exitosa] = "Alimentacion actualizado Exitosamente";

                    await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se actualizó la alimentacion '{alimentacion.Nombre}'", usuarioNombre);


                }
                await _unidadTrabajo.Guardar();
                return View("Index");
            }
            return View(alimentacion);
        }

        #region API
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var todos = await _unidadTrabajo.Alimentacion.ObtenerTodos();
            return Json(new { data = todos });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var alimentacionBd = await _unidadTrabajo.Alimentacion.Obtener(id);
            if (alimentacionBd == null)
            {
                return Json(new { success = false, message = "Error al borrar Alimentacion" });
            }

            var usuarioNombre = User.Identity.Name;

            //Remover imagen
            string upload = _webHostEnvironment.WebRootPath + DS.ImagenRutaAlimentacion;
            var anteriorFile = Path.Combine(upload, alimentacionBd.ImageUrl);
            if (System.IO.File.Exists(anteriorFile))
            {
                System.IO.File.Delete(anteriorFile);
            }

            try
            {
                _unidadTrabajo.Alimentacion.Remover(alimentacionBd);
                await _unidadTrabajo.Guardar();

                await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se eliminó la alimentacion '{alimentacionBd.Nombre}'", usuarioNombre);

                return Json(new { success = true, message = "Alimentacion borrado exitosamente" });
            }catch (Exception ex)
            {
                return Json(new { success = false, message = "Alimentacion asignada a un evento" });
            }
        }

        [ActionName("ValidarNombre")]
        public async Task<IActionResult> ValidarNombre(string nombre, int id = 0)
        {
            bool valor = false;
            var lista = await _unidadTrabajo.Alimentacion.ObtenerTodos();
            if (id == 0)
            {
                if(nombre != null)
                {
                    valor = lista.Any(b => b.Nombre.ToLower().Trim() == nombre.ToLower().Trim());
                }
            }
            else
            {
                if(nombre != null)
                {
                    valor = lista.Any(b => b.Nombre.ToLower().Trim() == nombre.ToLower().Trim() && b.Id != id);
                }
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
