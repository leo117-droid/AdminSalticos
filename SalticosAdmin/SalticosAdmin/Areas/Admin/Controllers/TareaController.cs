using Microsoft.AspNetCore.Mvc;
using SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio;
using SalticosAdmin.Modelos;
using SalticosAdmin.Utilidades;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace SalticosAdmin.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TareaController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;

        public TareaController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo = unidadTrabajo;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Upsert(int? id)
        {
            Tarea tarea = new Tarea();

            if (id == null)
            {
                return View(tarea);
            }

            tarea = await _unidadTrabajo.Tareas.Obtener(id.GetValueOrDefault());
            if (tarea == null)
            {
                return NotFound();
            }
            return View(tarea);

        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Tarea tarea)
        {
            if (ModelState.IsValid)
            {
                var usuarioNombre = User.Identity.Name;

                if (tarea.Id == 0)
                {
                    await _unidadTrabajo.Tareas.Agregar(tarea);
                    TempData[DS.Exitosa] = "Tarea creada exitosamente";
                    await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se agregó la tarea '{tarea.Titulo}'", usuarioNombre);

                }
                else
                {
                    // Editar tarea existente
                    _unidadTrabajo.Tareas.Actualizar(tarea);
                    TempData[DS.Exitosa] = "Tarea actualizada exitosamente";
                    await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se actualizó la tarea '{tarea.Titulo}'", usuarioNombre);

                }

                await _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));
            }
            TempData[DS.Error] = "Error al guardar Tarea";
            return View(tarea);
        }


        [HttpGet]
        public async Task<IActionResult> FiltrarPor(string criterio, string valor)
        {
            if (criterio == "Estado")
            {
                var tareas = await _unidadTrabajo.Tareas.FiltrarPorEstado(valor);
                return Json(new { data = tareas });
            }
            else if (criterio == "Prioridad")
            {
                var tareas = await _unidadTrabajo.Tareas.FiltrarPorPrioridad(valor);
                return Json(new { data = tareas });
            }

            return Json(new { success = false, message = "Criterio no válido" });
        }

        [HttpGet]
        public async Task<IActionResult> OrdenarPor(string criterio)
        {
            var tareas = await _unidadTrabajo.Tareas.ObtenerTodos();
            switch (criterio)
            {
                case "Prioridad":
                    tareas = tareas.OrderBy(t => t.Prioridad);
                    break;
                case "Estado":
                    tareas = tareas.OrderBy(t => t.Estado);
                    break;
                case "Fecha":
                    tareas = tareas.OrderBy(t => t.Fecha);
                    break;
                default:
                    return Json(new { success = false, message = "Criterio no válido" });
            }

            return Json(new { data = tareas });
        }
        [HttpPost]
        public async Task<IActionResult> ActualizarEstado(int id) 
        {
            var usuarioNombre = User.Identity.Name;
            var tarea = await _unidadTrabajo.Tareas.Obtener(id);

            if (tarea == null) 
            {
                return Json(new { success = false, message = "Tarea no encontrada" });
            }

            tarea.Estado = "Completada";
            _unidadTrabajo.Tareas.Actualizar(tarea);
            await _unidadTrabajo.Guardar();

            await _unidadTrabajo.Bitacora.RegistrarBitacora($"La tarea '{tarea.Titulo}' fue marcada como completada", usuarioNombre);

            return Json(new { success = true, message = "Tarea marcada como completada exitosamente" });
        }
        #region API
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var todos = await _unidadTrabajo.Tareas.ObtenerTodos();
            todos = todos.Where(t => !t.Estado.Equals("Completada"));
            return Json(new { data = todos });
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var usuarioNombre = User.Identity.Name;
            var tareaBD = await _unidadTrabajo.Tareas.Obtener(id);
            if (tareaBD == null)
            {
                return Json(new { success = false, message = "Tarea no encontrada" });
            }
            _unidadTrabajo.Tareas.Remover(tareaBD);
            await _unidadTrabajo.Guardar();
            await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se completó la tarea '{tareaBD.Titulo}'", usuarioNombre);

            return Json(new { success = true, message = "Tarea completada exitosamente" });
       
        }

        #endregion

    }
}
