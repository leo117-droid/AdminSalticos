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
        public List<Inflable> InflablesSeleccionados { get; set; } = new List<Inflable>();
        public List<Mobiliario> MobiliariosSeleccionados { get; set; } = new List<Mobiliario>();
        public List<ServicioAdicional> ServiciosSeleccionados { get; set; } = new List<ServicioAdicional>();
        public List<Alimentacion> AlimentacionSeleccionada { get; set; } = new List<Alimentacion>();
        public decimal MontoTotal { get; set; }

        public CotizacionController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo = unidadTrabajo;

        }

        public async Task<IActionResult> Index()
        {
            var inflables = await _unidadTrabajo.Inflable.ObtenerTodos();
            var mobiliarios = await _unidadTrabajo.Mobiliario.ObtenerTodos();
            var servicios = await _unidadTrabajo.ServicioAdicional.ObtenerTodos();
            var alimentacion = await _unidadTrabajo.Alimentacion.ObtenerTodos(); // Obtener la alimentación

            var modelo = new CotizacionVM
            {
                Inflables = (List<Inflable>)inflables,
                Mobiliarios = (List<Mobiliario>)mobiliarios,
                ServiciosAdicionales = (List<ServicioAdicional>)servicios,
                Alimentacion = (List<Alimentacion>)alimentacion // Agregar a la vista
            };

            return View(modelo);
        }

        public async Task<IActionResult> GenerarCotizacion(
            List<int> inflableIds,
            List<int> mobiliarioIds,
            List<int> cantidades,
            List<int> servicioIds,
            List<int> servicioCantidades,
            List<int> alimentacionIds, // IDs de alimentación seleccionada
            List<int> alimentacionCantidades) // Cantidades para cada opción de alimentación
        {
            if ((inflableIds == null || !inflableIds.Any()) &&
                (mobiliarioIds == null || !mobiliarioIds.Any()) &&
                (servicioIds == null || !servicioIds.Any()) &&
                (alimentacionIds == null || !alimentacionIds.Any()))
            {
                TempData["Error"] = "Debe seleccionar al menos un elemento.";
                return RedirectToAction(nameof(Index));
            }

            // Inflables
            var inflables = await _unidadTrabajo.Inflable.ObtenerTodos();
            var inflablesSeleccionados = inflables.Where(i => inflableIds.Contains(i.Id)).ToList();
            var montoInflables = inflablesSeleccionados.Sum(i => i.Precio);

            // Mobiliarios
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
                        var cantidad = cantidades.ElementAtOrDefault(i);
                        if (cantidad > 0)
                        {
                            mobiliariosSeleccionados.Add((mobiliario, cantidad));
                            montoMobiliarios += mobiliario.Precio * cantidad;
                        }
                    }
                }
            }

            // Servicios adicionales
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
                        var cantidad = servicioCantidades.ElementAtOrDefault(i);
                        if (cantidad > 0)
                        {
                            serviciosSeleccionados.Add((servicio, cantidad));
                            montoServicios += servicio.Precio * cantidad;
                        }
                    }
                }
            }

            // Alimentación
            var alimentacion = await _unidadTrabajo.Alimentacion.ObtenerTodos();
            var alimentacionSeleccionada = new List<(Alimentacion Alimentacion, int Cantidad)>();
            double montoAlimentacion = 0;

            if (alimentacionIds != null && alimentacionCantidades != null)
            {
                for (int i = 0; i < alimentacionIds.Count; i++)
                {
                    var opcion = alimentacion.FirstOrDefault(a => a.Id == alimentacionIds[i]);
                    if (opcion != null)
                    {
                        var cantidad = alimentacionCantidades.ElementAtOrDefault(i);
                        if (cantidad > 0)
                        {
                            alimentacionSeleccionada.Add((opcion, cantidad));
                            montoAlimentacion += opcion.Precio * cantidad;
                        }
                    }
                }
            }

            var cotizacion = new CotizacionVM
            {
                InflablesSeleccionados = inflablesSeleccionados.Any() ? inflablesSeleccionados : null,
                MobiliariosSeleccionados = mobiliariosSeleccionados.Any() ? mobiliariosSeleccionados : null,
                ServiciosSeleccionados = serviciosSeleccionados.Any() ? serviciosSeleccionados : null,
                AlimentacionSeleccionada = alimentacionSeleccionada.Any() ? alimentacionSeleccionada : null,
                MontoTotal = montoInflables + montoMobiliarios + montoServicios + montoAlimentacion
            };

            return View("ResumenCotizacion", cotizacion);
        }



    }
}
