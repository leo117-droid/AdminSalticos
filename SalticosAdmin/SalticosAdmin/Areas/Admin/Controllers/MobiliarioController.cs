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
    public class MobiliarioController : Controller
    {

        private readonly IUnidadTrabajo _unidadTrabajo;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MobiliarioController(IUnidadTrabajo unidadTrabajo, IWebHostEnvironment webHostEnvironment)
        {
            _unidadTrabajo = unidadTrabajo;
            _webHostEnvironment = webHostEnvironment;

        }
        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> Upsert(int? id)
        {
            Mobiliario mobiliario = new Mobiliario();


            if (id == null)
            {
                // Crear una nuevo mobiliario
                return View(mobiliario);
            }

            // Actualizamos
            mobiliario = await _unidadTrabajo.Mobiliario.Obtener(id.GetValueOrDefault());
            if (mobiliario == null)
            {
                return NotFound();
            }
            return View(mobiliario);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Mobiliario mobiliario)
        {
            if (ModelState.IsValid)
            {
                var usuarioNombre = User.Identity.Name;

                var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnvironment.WebRootPath;
                if (mobiliario.Id == 0)
                {

                    string upload = webRootPath + DS.ImagenRutaMobiliario;
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);
                    using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }
                    mobiliario.ImageUrl = fileName + extension;


                    await _unidadTrabajo.Mobiliario.Agregar(mobiliario);
                    TempData[DS.Exitosa] = "Mobiliario creado Exitosamente";

                    await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se agregó el mobiliario '{mobiliario.Nombre}'", usuarioNombre);

                }
                else
                {
                    var objMobiliario = await _unidadTrabajo.Mobiliario.ObtenerPrimero(p => p.Id == mobiliario.Id, isTracking: false);
                    if (files.Count > 0) // Si se carga una nueva imagen
                    {
                        string upload = webRootPath + DS.ImagenRutaMobiliario;
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);

                        // Borrar la imagen anterior
                        var anteriorFile = Path.Combine(upload, objMobiliario.ImageUrl);
                        if (System.IO.File.Exists(anteriorFile))
                        {
                            System.IO.File.Delete(anteriorFile);
                        }
                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }
                        mobiliario.ImageUrl = fileName + extension;
                    } // Caso contrario no se carga una nueva imagen
                    else
                    {
                        mobiliario.ImageUrl = objMobiliario.ImageUrl;
                    }

                    _unidadTrabajo.Mobiliario.Actualizar(mobiliario);
                    TempData[DS.Exitosa] = "Mobiliario actualizado Exitosamente";

                    await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se actualizó el mobiliario '{mobiliario.Nombre}'", usuarioNombre);

                }
                await _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));
            }
            TempData[DS.Error] = "Error al grabar mobiliario";
            return View(mobiliario);
        }


        #region API
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var todos = await _unidadTrabajo.Mobiliario.ObtenerTodos();
            return Json(new { data = todos });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var mobiliarioBd = await _unidadTrabajo.Mobiliario.Obtener(id);

            var usuarioNombre = User.Identity.Name;

            if (mobiliarioBd == null)
            {
                return Json(new { success = false, message = "Error al borrar Mobiliario" });
            }

            //Remover imagen
            string upload = _webHostEnvironment.WebRootPath + DS.ImagenRutaMobiliario;
            var anteriorFile = Path.Combine(upload, mobiliarioBd.ImageUrl);
            if (System.IO.File.Exists(anteriorFile))
            {
                System.IO.File.Delete(anteriorFile);
            }

            try
            {
                _unidadTrabajo.Mobiliario.Remover(mobiliarioBd);
                await _unidadTrabajo.Guardar();

                await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se eliminó el mobiliario '{mobiliarioBd.Nombre}'", usuarioNombre);

                return Json(new { success = true, message = "Mobiliario borrada exitosamente" });
            } catch (Exception ex)
            {
                return Json(new { success = false, message = "Mobiliario asignado a un evento" });
            }
        }

        [ActionName("ValidarNombre")]
        public async Task<IActionResult> ValidarNombre(string nombre, int id = 0)
        {
            bool valor = false;
            var lista = await _unidadTrabajo.Mobiliario.ObtenerTodos();
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
