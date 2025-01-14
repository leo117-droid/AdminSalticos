using Microsoft.AspNetCore.Mvc;
using SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio;
using SalticosAdmin.Modelos;
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
        public IActionResult Upsert(int? id)
        {
            if (id == null || id == 0)
            {
                // Crear nueva tarea
                return View(new Tarea());
            }
            else
            {
                // Editar tarea existente
                var tarea = _unidadTrabajo.Tareas.Obtener(id.GetValueOrDefault());
                if (tarea == null)
                {
                    return NotFound();
                }
                return View(tarea);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Tarea tarea)
        {
            if (ModelState.IsValid)
            {
                if (tarea.Id == 0)
                {
                    // Crear nueva tarea
                    await _unidadTrabajo.Tareas.Agregar(tarea);
                    TempData["Success"] = "Tarea creada exitosamente";
                }
                else
                {
                    // Editar tarea existente
                    _unidadTrabajo.Tareas.Actualizar(tarea);
                    TempData["Success"] = "Tarea actualizada exitosamente";
                }

                await _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));
            }
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

        [HttpPost]
        public async Task<IActionResult> CrearTarea([FromBody] Tarea tarea)
        {
            if (ModelState.IsValid)
            {
                await _unidadTrabajo.Tareas.Agregar(tarea);
                await _unidadTrabajo.Guardar();
                return Json(new { success = true, message = "Tarea creada exitosamente" });
            }

            return Json(new { success = false, message = "Datos no válidos" });
        }

        [HttpPost]
        public async Task<IActionResult> Editar([FromBody] Tarea tarea)
        {
            if (ModelState.IsValid)
            {
                _unidadTrabajo.Tareas.Actualizar(tarea);
                await _unidadTrabajo.Guardar();
                return Json(new { success = true, message = "Tarea actualizada exitosamente" });
            }

            return Json(new { success = false, message = "Datos no válidos" });
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

        #region API
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var todos = await _unidadTrabajo.Tareas.ObtenerTodos();
            //return Json(new
            //{
            //    data = todos.Select(t => new
            //    {
            //        t.Titulo,
            //        t.Descripcion,
            //        t.Estado,
            //        t.Prioridad,
            //        Fecha = t.Fecha.ToString("yyyy-MM-dd"), // Opcional: formatear fecha
            //        Hora = t.Hora.ToString(@"hh\:mm")
            //    })
            //});
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
            await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se eliminó la tarea '{tareaBD.Titulo}'", usuarioNombre);
            return Json(new { success = true, message = "Tarea eliminada exitosamente" });
       
        }

        #endregion

    }
}
