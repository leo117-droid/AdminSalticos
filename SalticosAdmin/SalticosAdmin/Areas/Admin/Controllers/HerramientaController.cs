using Microsoft.AspNetCore.Mvc;
using SalticosAdmin.AccesoDeDatos.Repositorio;
using SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio;

namespace SalticosAdmin.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HerramientaController : Controller
    {

        private readonly IUnidadTrabajo _unidadTrabajo;
        public HerramientaController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo = unidadTrabajo;
        }
        public IActionResult Index()
        {
            return View();
        }

        #region API
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var todos = await _unidadTrabajo.Herramienta.ObtenerTodos();
            return Json(new { data = todos });
        }



        #endregion
    }
}
