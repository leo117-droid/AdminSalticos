using Microsoft.AspNetCore.Mvc;
using SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio;
using SalticosAdmin.Modelos;
using SalticosAdmin.Modelos.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SalticosAdmin.Servicios;
using System.IO;
using System.Collections.Generic;
using iTextSharp.text;
using iTextSharp.text.pdf;

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
            var tarifas = await _unidadTrabajo.TarifasTransporte.ObtenerTodos();

            var modelo = new CotizacionVM
            {
                Inflables = (List<Inflable>)inflables,
                Mobiliarios = (List<Mobiliario>)mobiliarios,
                ServiciosAdicionales = (List<ServicioAdicional>)servicios,
                Alimentacion = (List<Alimentacion>)alimentacion, // Agregar a la vista
                TarifasTransporte = (List<TarifasTransporte>)tarifas // Agregar a la vista

            };

            return View(modelo);
        }

        public async Task<IActionResult> GenerarCotizacion(
            List<int> inflableIds,
            List<int> inflableHorasAdicionales, // Horas adicionales por inflable
            List<int> mobiliarioIds,
            List<int> cantidades,
            List<int> servicioIds,
            List<int> servicioCantidades,
            List<int> alimentacionIds, // IDs de alimentación seleccionada
            List<int> alimentacionCantidades, 
            List<int> transporteIds) // Cantidades para cada opción de alimentación
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
            var inflablesSeleccionados = new List<(Inflable Inflable, int HorasAdicionales)>();
            double montoInflables = 0;

            if (inflableIds != null && inflableHorasAdicionales != null)
            {
                for (int i = 0; i < inflableIds.Count; i++)
                {
                    var inflable = inflables.FirstOrDefault(m => m.Id == inflableIds[i]);
                    if (inflable != null)
                    {
                        // Si las horas adicionales no están especificadas, asigna 0 por defecto.
                        var horaAdicional = inflableHorasAdicionales.ElementAtOrDefault(i);

                        // Asegúrate de que horaAdicional no sea negativo.
                        horaAdicional = Math.Max(0, horaAdicional);

                        // Agrega el inflable a la lista de seleccionados.
                        inflablesSeleccionados.Add((inflable, horaAdicional));

                        // Calcula el monto del inflable, incluyendo las horas adicionales (si las hay).
                        montoInflables += inflable.Precio + (inflable.PrecioHoraAdicional * horaAdicional);
                    }
                }
            }


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

            //Tarifas de Transporte
            var transportes = await _unidadTrabajo.TarifasTransporte.ObtenerTodos();
            var tarifaTransporteSeleccionada = transportes.Where(i => transporteIds.Contains(i.Id)).ToList();
            var montoTransporte = tarifaTransporteSeleccionada.Sum(i => i.Precio);


            var cotizacion = new CotizacionVM
            {
                InflablesSeleccionados = inflablesSeleccionados.Any() ? inflablesSeleccionados : null,
                MobiliariosSeleccionados = mobiliariosSeleccionados.Any() ? mobiliariosSeleccionados : null,
                ServiciosSeleccionados = serviciosSeleccionados.Any() ? serviciosSeleccionados : null,
                AlimentacionSeleccionada = alimentacionSeleccionada.Any() ? alimentacionSeleccionada : null,
                TarifaTransporteSeleccionada = tarifaTransporteSeleccionada.Any() ? tarifaTransporteSeleccionada: null, 
                MontoTotal = montoInflables + montoMobiliarios + montoServicios + montoAlimentacion + montoTransporte
            };

            return View("ResumenCotizacion", cotizacion);
        }

        public async Task<IActionResult> DescargarPDF(
            List<int> inflableIds,
            List<int> mobiliarioIds,
            List<int> servicioIds,
            List<int> alimentacionIds,
            List<int> transporteIds)
        {
            var inflables = await _unidadTrabajo.Inflable.ObtenerTodos();
            var mobiliarios = await _unidadTrabajo.Mobiliario.ObtenerTodos();
            var servicios = await _unidadTrabajo.ServicioAdicional.ObtenerTodos();
            var alimentacion = await _unidadTrabajo.Alimentacion.ObtenerTodos();
            var tarifas = await _unidadTrabajo.TarifasTransporte.ObtenerTodos();

            var inflablesSeleccionados = inflables.Where(i => inflableIds.Contains(i.Id)).ToList();
            var mobiliariosSeleccionados = mobiliarios.Where(m => mobiliarioIds.Contains(m.Id)).ToList();
            var serviciosSeleccionados = servicios.Where(s => servicioIds.Contains(s.Id)).ToList();
            var alimentacionSeleccionada = alimentacion.Where(a => alimentacionIds.Contains(a.Id)).ToList();
            var transporteSeleccionado = tarifas.Where(t => transporteIds.Contains(t.Id)).ToList();

            using (var ms = new MemoryStream())
            {
                var document = new Document();
                PdfWriter.GetInstance(document, ms);
                document.Open();

                document.Add(new Paragraph("Resumen de Cotización"));
                document.Add(new Paragraph(" "));

                if (inflablesSeleccionados.Any())
                {
                    document.Add(new Paragraph("Inflables Seleccionados"));
                    foreach (var inflable in inflablesSeleccionados)
                    {
                        document.Add(new Paragraph($"Nombre: {inflable.Nombre}, Descripción: {inflable.Descripcion}, Precio: ₡{inflable.Precio.ToString("N2")}"));
                    }
                }

                document.Add(new Paragraph(" "));

                if (mobiliariosSeleccionados.Any())
                {
                    document.Add(new Paragraph("Mobiliarios Seleccionados"));
                    foreach (var mobiliario in mobiliariosSeleccionados)
                    {
                        document.Add(new Paragraph($"Nombre: {mobiliario.Nombre}, Descripción: {mobiliario.Descripcion}, Precio: ₡{mobiliario.Precio.ToString("N2")}"));
                    }
                }

                document.Add(new Paragraph(" "));

                if (serviciosSeleccionados.Any())
                {
                    document.Add(new Paragraph("Servicios Adicionales Seleccionados"));
                    foreach (var servicio in serviciosSeleccionados)
                    {
                        document.Add(new Paragraph($"Nombre: {servicio.Nombre}, Descripción: {servicio.Descripcion}, Precio: ₡{servicio.Precio.ToString("N2")}"));
                    }
                }

                document.Add(new Paragraph(" "));

                if (alimentacionSeleccionada.Any())
                {
                    document.Add(new Paragraph("Alimentación Seleccionada"));
                    foreach (var opcion in alimentacionSeleccionada)
                    {
                        document.Add(new Paragraph($"Nombre: {opcion.Nombre}, Descripción: {opcion.Descripcion}, Precio: ₡{opcion.Precio.ToString("N2")}"));
                    }
                }

                document.Add(new Paragraph(" "));

                if (transporteSeleccionado.Any())
                {
                    document.Add(new Paragraph("Transporte Seleccionado"));
                    foreach (var tarifa in transporteSeleccionado)
                    {
                        document.Add(new Paragraph($"Provincia: {tarifa.Provincia}, Precio: ₡{tarifa.Precio.ToString("N2")}"));
                    }
                }

                document.Close();
                var bytes = ms.ToArray();
                return File(bytes, "application/pdf", "ResumenCotizacion.pdf");
            }
        }




    }
}
