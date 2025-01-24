using Microsoft.AspNetCore.Mvc;
using SalticosAdmin.AccesoDeDatos.Repositorio;
using SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio;
using SalticosAdmin.Modelos;
using SalticosAdmin.Utilidades;

namespace SalticosAdmin.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BitacoraController : Controller
    {

        private readonly IUnidadTrabajo _unidadTrabajo;

        public BitacoraController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo = unidadTrabajo;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Intermedia()
        {
            return View();
        }

        #region API
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var todos = await _unidadTrabajo.Bitacora.ObtenerTodos();
            var ordenados = todos.OrderByDescending(b => b.Fecha).ToList();
            return Json(new { data = ordenados });
        }

       


        [HttpGet]
        public async Task<IActionResult> ConsultarConFiltro(DateTime fechainicial, DateTime fechafinal)

        {
            if (fechainicial != DateTime.MinValue && fechafinal != DateTime.MinValue)
            {
                var registrosBitacora = await _unidadTrabajo.Bitacora.ObtenerEntreFechas(fechainicial, fechafinal);
                return Json(new { data = registrosBitacora });

            }

            var Todos = await _unidadTrabajo.Bitacora.ObtenerTodos();
            return Json(new { data = Todos });
        }
        #endregion

    }

}

