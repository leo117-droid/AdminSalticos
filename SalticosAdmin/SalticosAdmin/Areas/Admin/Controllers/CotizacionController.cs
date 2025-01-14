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
            var servicios = await _unidadTrabajo.ServicioAdicional.ObtenerTodos();


            // Crear el modelo fuertemente tipado para la vista
            var modelo = new CotizacionVM
            {
                Inflables = (List<Inflable>)inflables,
                Mobiliarios = (List<Mobiliario>)mobiliarios,
                ServiciosAdicionales = (List<ServicioAdicional>)servicios
            };

            return View(modelo);
        }

        public async Task<IActionResult> GenerarCotizacion(
            List<int> inflableIds,
            List<int> mobiliarioIds,
            List<int> cantidades,
            List<int> servicioIds,
            List<int> servicioCantidades) // Cantidades para los servicios seleccionados
        {
            if ((inflableIds == null || !inflableIds.Any()) &&
                (mobiliarioIds == null || !mobiliarioIds.Any()) &&
                (servicioIds == null || !servicioIds.Any()))
            {
                TempData["Error"] = "Debe seleccionar al menos un inflable, mobiliario o servicio adicional.";
                return RedirectToAction(nameof(Index));
            }

            // Obtener inflables seleccionados
            var inflables = await _unidadTrabajo.Inflable.ObtenerTodos();
            var inflablesSeleccionados = inflables
                .Where(i => inflableIds.Contains(i.Id))
                .ToList();
            var montoInflables = inflablesSeleccionados.Sum(i => i.Precio);

            // Obtener mobiliarios seleccionados y calcular el monto
            var mobiliarios = await _unidadTrabajo.Mobiliario.ObtenerTodos();
            var mobiliariosSeleccionados = new List<(Mobiliario Mobiliario, int Cantidad)>();
            double montoMobiliarios = 0;

            if (mobiliarioIds != null && cantidades != null)
            {
                for (int i = 0; i < mobiliarioIds.Count; i++)
                {
                    var mobiliario = mobiliarios.FirstOrDefault(m => m.Id == mobiliarioIds[i]);
                    if (mobiliario != null)
                    {
                        var cantidad = cantidades.ElementAtOrDefault(i); // Evitar excepciones si la cantidad no existe
                        if (cantidad > 0) // Solo procesar si la cantidad es válida
                        {
                            mobiliariosSeleccionados.Add((mobiliario, cantidad));
                            montoMobiliarios += mobiliario.Precio * cantidad;
                        }
                    }
                }
            }

            // Obtener servicios adicionales seleccionados y calcular el monto
            var servicios = await _unidadTrabajo.ServicioAdicional.ObtenerTodos();
            var serviciosSeleccionados = new List<(ServicioAdicional Servicio, int Cantidad)>();
            double montoServicios = 0;

            if (servicioIds != null && servicioCantidades != null)
            {
                for (int i = 0; i < servicioIds.Count; i++)
                {
                    var servicio = servicios.FirstOrDefault(s => s.Id == servicioIds[i]);
                    if (servicio != null)
                    {
                        var cantidad = servicioCantidades.ElementAtOrDefault(i); // Evitar excepciones si la cantidad no existe
                        if (cantidad > 0) // Solo procesar si la cantidad es válida
                        {
                            serviciosSeleccionados.Add((servicio, cantidad));
                            montoServicios += servicio.Precio * cantidad;
                        }
                    }
                }
            }

            // Crear el modelo de cotización
            var cotizacion = new CotizacionVM
            {
                InflablesSeleccionados = inflablesSeleccionados,
                MobiliariosSeleccionados = mobiliariosSeleccionados,
                ServiciosSeleccionados = serviciosSeleccionados,
                MontoTotal = montoInflables + montoMobiliarios + montoServicios
            };

            return View("ResumenCotizacion", cotizacion);
        }


    }
}
