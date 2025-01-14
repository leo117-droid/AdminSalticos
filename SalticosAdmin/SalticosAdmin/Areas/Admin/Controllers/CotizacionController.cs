using Microsoft.AspNetCore.Mvc;
using SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio;
using SalticosAdmin.Modelos;
using System.Linq;
using System.Threading.Tasks;

namespace SalticosAdmin.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CotizacionController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;

        public CotizacionController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo = unidadTrabajo;
        }

        // Vista principal donde se listan los inflables para seleccionarlos
        public async Task<IActionResult> Index()
        {
            // Obtener todos los inflables disponibles
            var inflables = await _unidadTrabajo.Inflable.ObtenerTodos();
            return View(inflables);
        }

        // Acción para generar la cotización
        [HttpPost]
        public async Task<IActionResult> GenerarCotizacion(List<int> inflableIds)
        {
            // Validar que se hayan seleccionado inflables
            if (inflableIds == null || !inflableIds.Any())
            {
                TempData["Error"] = "Debe seleccionar al menos un inflable.";
                return RedirectToAction(nameof(Index));
            }

            // Obtener los inflables seleccionados
            var inflables = await _unidadTrabajo.Inflable.ObtenerTodos();
            var inflablesSeleccionados = inflables.Where(i => inflableIds.Contains(i.Id)).ToList();

            // Calcular el monto total
            var montoTotal = inflablesSeleccionados.Sum(i => i.Precio);

            // Crear el modelo de cotización
            var cotizacion = new Cotizacion
            {
                InflableIds = inflableIds,
                InflablesSeleccionados = inflablesSeleccionados,
                MontoTotal = montoTotal
            };

            // Pasar el modelo a la vista de resumen
            return View("ResumenCotizacion", cotizacion);
        }
    }
}
