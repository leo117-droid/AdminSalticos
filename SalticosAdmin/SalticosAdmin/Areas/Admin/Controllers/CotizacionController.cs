using Microsoft.AspNetCore.Mvc;
using SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio;
using SalticosAdmin.Modelos;
using SalticosAdmin.Modelos.ViewModels;
using System.Collections.Generic;
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

        public async Task<IActionResult> Index()
        {
            // Obtener todos los inflables y mobiliarios
            var inflables = await _unidadTrabajo.Inflable.ObtenerTodos();
            var mobiliarios = await _unidadTrabajo.Mobiliario.ObtenerTodos();

            // Crear el modelo fuertemente tipado para la vista
            var modelo = new CotizacionVM
            {
                Inflables = (List<Inflable>)inflables,
                Mobiliarios = (List<Mobiliario>)mobiliarios
            };

            return View(modelo);
        }

        public async Task<IActionResult> GenerarCotizacion(
            List<int> inflableIds,
            List<int> mobiliarioIds,
            List<int> cantidades)
        {
            // Validar que se haya seleccionado al menos un inflable o mobiliario
            if ((inflableIds == null || !inflableIds.Any()) &&
                (mobiliarioIds == null || !mobiliarioIds.Any()))
            {
                TempData["Error"] = "Debe seleccionar al menos un inflable o mobiliario.";
                return RedirectToAction(nameof(Index));
            }

            // Obtener los inflables seleccionados
            var inflables = await _unidadTrabajo.Inflable.ObtenerTodos();
            var inflablesSeleccionados = inflables
                .Where(i => inflableIds.Contains(i.Id))
                .ToList();

            // Calcular el monto total de inflables
            var montoInflables = inflablesSeleccionados.Sum(i => i.Precio);

            // Obtener los mobiliarios seleccionados y asociar la cantidad
            var mobiliarios = await _unidadTrabajo.Mobiliario.ObtenerTodos();
            var mobiliariosSeleccionados = new List<(Mobiliario Mobiliario, int Cantidad)>();
            double montoMobiliarios = 0;

            if (mobiliarioIds != null && cantidades != null && mobiliarioIds.Count == cantidades.Count)
            {
                for (int i = 0; i < mobiliarioIds.Count; i++)
                {
                    var mobiliario = mobiliarios.FirstOrDefault(m => m.Id == mobiliarioIds[i]);
                    if (mobiliario != null)
                    {
                        // Añadir la cantidad junto al mobiliario
                        mobiliariosSeleccionados.Add((mobiliario, cantidades[i]));

                        // Calcular el monto total de mobiliarios
                        montoMobiliarios += mobiliario.Precio * cantidades[i];
                    }
                }
            }

            var cotizacion = new CotizacionVM
            {
                InflablesSeleccionados = inflablesSeleccionados,
                MobiliariosSeleccionados = mobiliariosSeleccionados,
                MontoTotal = montoInflables + montoMobiliarios
            };

            // Pasar el modelo a la vista de resumen
            return View("ResumenCotizacion", cotizacion);
        }
    }
    }
