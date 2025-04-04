﻿using Microsoft.AspNetCore.Mvc;
using SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Identity.UI.Services;
using SalticosAdmin.Modelos;
using SalticosAdmin.Modelos.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using System.Data.SqlTypes;
using static System.Formats.Asn1.AsnWriter;
using System.Net.Mail;
using MailKit.Net.Smtp;
using MimeKit;
using Newtonsoft.Json;
using System.Text;
using SalticosAdmin.Utilidades;

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
        public CotizacionVM cotizacionActual { get; set; }

        private readonly IServiceProvider _serviceProvider;

        public CotizacionController(IUnidadTrabajo unidadTrabajo, IServiceProvider serviceProvider)
        {
            _unidadTrabajo = unidadTrabajo;
            _serviceProvider = serviceProvider;

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
            List<int> mobiliarioCantidades,
            List<int> servicioIds,
            List<int> servicioCantidades,
            List<int> alimentacionIds, // IDs de alimentación seleccionada
            List<int> alimentacionCantidades,
            List<int> transporteIds,
            IFormCollection form)
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

                        /// ------ PRUEBA DE HORAS ADICIONALES

                        // Busca las horas adicionales asociadas al inflable
                        var key = $"inflableHorasAdicionales_{inflableIds[i]}";
                        var horasAdicionalesStr = form[key];
                        int horaAdicional = 0;

                        // Si se encuentra el valor y es válido, úsalo; de lo contrario, asigna 0
                        if (!string.IsNullOrEmpty(horasAdicionalesStr) && int.TryParse(horasAdicionalesStr, out var horas))
                        {
                            horaAdicional = Math.Max(0, horas);
                        }



     

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

            if (mobiliarioIds != null && mobiliarioCantidades != null)
            {
                for (int i = 0; i < mobiliarioIds.Count; i++)
                {
                    var mobiliario = mobiliarios.FirstOrDefault(m => m.Id == mobiliarioIds[i]);
                    if (mobiliario != null)
                    {
                        // Busca la cantidad asociada al mobiliario
                        var key = $"mobiliarioCantidades_{mobiliarioIds[i]}";
                        var cantidadStr = form[key];
                        int cantidad = 0;

                        // Si se encuentra el valor y es válido, úsalo; de lo contrario, asigna 0
                        if (!string.IsNullOrEmpty(cantidadStr) && int.TryParse(cantidadStr, out var cantidadParseada))
                        {
                            cantidad = Math.Max(0, cantidadParseada);
                        }
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
                        // Busca la cantidad asociada al servicio
                        var key = $"servicioCantidades_{servicioIds[i]}";
                        var cantidadStr = form[key];
                        int cantidad = 0;

                        // Valida y asigna la cantidad
                        if (!string.IsNullOrEmpty(cantidadStr) && int.TryParse(cantidadStr, out var cantidadParseada))
                        {
                            cantidad = Math.Max(1, cantidadParseada); // Mínimo 1
                        }
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
                        // Busca la cantidad asociada al alimento
                        var key = $"alimentacionCantidades_{alimentacionIds[i]}";
                        var cantidadStr = form[key];
                        int cantidad = 0;

                        // Si se encuentra el valor y es válido, úsalo; de lo contrario, asigna 0
                        if (!string.IsNullOrEmpty(cantidadStr) && int.TryParse(cantidadStr, out var cantidadParseada))
                        {
                            cantidad = Math.Max(0, cantidadParseada);
                        }
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
                TarifaTransporteSeleccionada = tarifaTransporteSeleccionada.Any() ? tarifaTransporteSeleccionada : null,
                MontoTotal = montoInflables + montoMobiliarios + montoServicios + montoAlimentacion + montoTransporte
            };

            TempData["CotizacionActual"] = JsonConvert.SerializeObject(cotizacion);
            TempData["CotizacionActualCorreo"] = JsonConvert.SerializeObject(cotizacion);
            TempData.Keep("CotizacionActual");
            TempData.Keep("CotizacionActualCorreo");
            return View("ResumenCotizacion", cotizacion);
        }


        public byte[] CrearPDF(CotizacionVM cotizacionActual)
        {
            using (var memoryStream = new MemoryStream())
            {
                var document = new Document(PageSize.A4, 36, 36, 54, 54);
                var writer = PdfWriter.GetInstance(document, memoryStream);
                document.Open();
                AddHeaderAndFooter(document, writer);

                var baseFontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf");
                var baseFont = BaseFont.CreateFont(baseFontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);



                // Título principal

                var titleFont = new Font(baseFont, 18, Font.BOLD);
                var sectionFont = new Font(baseFont, 14, Font.BOLD);
                var tableHeaderFont = new Font(baseFont, 12, Font.BOLD);
                var tableCellFont = new Font(baseFont, 12);

                

                document.Add(new Paragraph("Cotización Sal-Ticos", titleFont) { Alignment = Element.ALIGN_CENTER });
                document.Add(new Paragraph(" "));

                var montoTotalParagraph = new Paragraph();
                montoTotalParagraph.Add(new Chunk("Monto total: ", new Font(baseFont, 12, Font.BOLD))); // Texto en negrita
                montoTotalParagraph.Add(new Chunk($"₡{cotizacionActual.MontoTotal:N2}", new Font(baseFont, 12))); // Texto normal
                document.Add(montoTotalParagraph);

                var montoTotalIVAParagraph = new Paragraph();
                montoTotalIVAParagraph.Add(new Chunk("Monto total con IVA: ", new Font(baseFont, 12, Font.BOLD))); // Texto en negrita
                montoTotalIVAParagraph.Add(new Chunk($"₡{cotizacionActual.MontoTotalIVA:N2}", new Font(baseFont, 12))); // Texto normal
                document.Add(montoTotalIVAParagraph);

                document.Add(new Paragraph(" "));

                // Métodos para tablas
                PdfPTable CreateTable(int columnCount, float[] columnWidths)
                {
                    var table = new PdfPTable(columnCount) { WidthPercentage = 100 };
                    table.SetWidths(columnWidths);
                    return table;
                }

                void AddTableHeaders(PdfPTable table, string[] headers)
                {
                    foreach (var header in headers)
                    {
                        var cell = new PdfPCell(new Phrase(header, tableHeaderFont))
                        {
                            BackgroundColor = BaseColor.LIGHT_GRAY,
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            Padding = 5
                        };
                        table.AddCell(cell);
                    }
                }

                void AddCell(PdfPTable table, string content, bool isNumeric = false)
                {
                    var cell = new PdfPCell(new Phrase(content, tableCellFont))
                    {
                        HorizontalAlignment = isNumeric ? Element.ALIGN_RIGHT : Element.ALIGN_CENTER,
                        Padding = 5
                    };
                    table.AddCell(cell);
                }

                // Inflables seleccionados
                if (cotizacionActual.InflablesSeleccionados?.Any() == true)
                {
                    document.Add(new Paragraph("Inflables seleccionados:", sectionFont));
                    document.Add(new Paragraph(" "));

                    var table = CreateTable(6, new float[] { 2, 3, 2, 2, 2, 2 });
                    AddTableHeaders(table, new[] { "Nombre", "Descripción", "Precio base", "Precio / hora adicional", "Horas adicionales", "Precio total" });

                    foreach (var inflable in cotizacionActual.InflablesSeleccionados)
                    {
                        AddCell(table, inflable.Inflable.Nombre);
                        AddCell(table, inflable.Inflable.Descripcion);
                        AddCell(table, $"₡{inflable.Inflable.Precio:N2}", true);
                        AddCell(table, $"₡{inflable.Inflable.PrecioHoraAdicional:N2}", true);
                        AddCell(table, inflable.HorasAdicionales.ToString(), false);
                        var precioTotal = inflable.Inflable.Precio + inflable.HorasAdicionales * inflable.Inflable.PrecioHoraAdicional;
                        AddCell(table, $"₡{precioTotal:N2}", true);
                    }
                    document.Add(table);
                    document.Add(new Paragraph(" "));
                }

                // Mobiliarios seleccionados
                if (cotizacionActual.MobiliariosSeleccionados?.Any() == true)
                {
                    document.Add(new Paragraph("Mobiliarios seleccionados:", sectionFont));
                    document.Add(new Paragraph(" "));

                    var table = CreateTable(4, new float[] { 3, 2, 2, 2 });
                    AddTableHeaders(table, new[] { "Nombre", "Descripción", "Cantidad", "Precio total" });

                    foreach (var mobiliario in cotizacionActual.MobiliariosSeleccionados)
                    {
                        AddCell(table, mobiliario.Mobiliario.Nombre);
                        AddCell(table, mobiliario.Mobiliario.Descripcion);
                        AddCell(table, mobiliario.Cantidad.ToString(), false);
                        var precioTotal = mobiliario.Mobiliario.Precio * mobiliario.Cantidad;
                        AddCell(table, $"₡{precioTotal:N2}", true);
                    }
                    document.Add(table);
                    document.Add(new Paragraph(" "));
                }

                // Servicios seleccionados
                if (cotizacionActual.ServiciosSeleccionados?.Any() == true)
                {
                    document.Add(new Paragraph("Servicios adicionales seleccionados:", sectionFont));
                    document.Add(new Paragraph(" "));

                    var table = CreateTable(4, new float[] { 3, 2, 2, 2 });
                    AddTableHeaders(table, new[] { "Servicio", "Precio / unidad", "Cantidad", "Precio total" });

                    foreach (var servicio in cotizacionActual.ServiciosSeleccionados)
                    {
                        AddCell(table, servicio.Servicio.Nombre);
                        AddCell(table, $"₡{servicio.Servicio.Precio:N2}", true);
                        AddCell(table, servicio.Cantidad.ToString(), false);
                        var precioTotal = servicio.Servicio.Precio * servicio.Cantidad;
                        AddCell(table, $"₡{precioTotal:N2}", true);
                    }
                    document.Add(table);
                    document.Add(new Paragraph(" "));
                }

                // Alimentación seleccionada
                if (cotizacionActual.AlimentacionSeleccionada?.Any() == true)
                {
                    document.Add(new Paragraph("Alimentación seleccionada:", sectionFont));
                    document.Add(new Paragraph(" "));

                    var table = CreateTable(4, new float[] { 3, 2, 2, 2 });
                    AddTableHeaders(table, new[] { "Alimento", "Precio / unidad", "Cantidad", "Precio total" });

                    foreach (var opcion in cotizacionActual.AlimentacionSeleccionada)
                    {
                        AddCell(table, opcion.Alimentacion.Nombre);
                        AddCell(table, $"₡{opcion.Alimentacion.Precio:N2}", true);
                        AddCell(table, opcion.Cantidad.ToString(), false);
                        var precioTotal = opcion.Alimentacion.Precio * opcion.Cantidad;
                        AddCell(table, $"₡{precioTotal:N2}", true);
                    }
                    document.Add(table);
                    document.Add(new Paragraph(" "));
                }

                // Transporte seleccionado
                if (cotizacionActual.TarifaTransporteSeleccionada?.Any() == true)
                {
                    document.Add(new Paragraph("Transporte seleccionado:", sectionFont));
                    document.Add(new Paragraph(" "));

                    var table = CreateTable(2, new float[] { 3, 2 });
                    AddTableHeaders(table, new[] { "Provincia", "Precio" });

                    foreach (var tarifa in cotizacionActual.TarifaTransporteSeleccionada)
                    {
                        AddCell(table, tarifa.Provincia);
                        AddCell(table, $"₡{tarifa.Precio:N2}", true);
                    }
                    document.Add(table);
                    document.Add(new Paragraph(" "));
                }

                document.Close();
                return memoryStream.ToArray();
            }
        }

        [HttpPost]
        public IActionResult DescargarPDF()
        {
            if (TempData["CotizacionActual"] == null)
            {
                TempData["Error"] = "No se encontró una cotización activa.";
                return RedirectToAction(nameof(Index));
            }

            var cotizacionJson = TempData["CotizacionActual"].ToString();
            var cotizacionActual = JsonConvert.DeserializeObject<CotizacionVM>(cotizacionJson);

            if (cotizacionActual == null)
            {
                TempData["Error"] = "Error al procesar los datos de la cotización.";
                return RedirectToAction(nameof(Index));
            }

            var pdfContent = CrearPDF(cotizacionActual);
            TempData["CotizacionActual"] = JsonConvert.SerializeObject(cotizacionActual);
            TempData["CotizacionActualCorreo"] = JsonConvert.SerializeObject(cotizacionActual);
            TempData.Keep("CotizacionActual");
            TempData.Keep("CotizacionActualCorreo");
            return File(pdfContent, "application/pdf", "Cotizacion.pdf");
        }

        [HttpPost]
        public async Task<IActionResult> EnviarCotizacionPorCorreo(string correo)
        {
            var cotizacionJson = TempData["CotizacionActualCorreo"].ToString();
            var cotizacionActual = JsonConvert.DeserializeObject<CotizacionVM>(cotizacionJson);

            // Construir el mensaje en formato HTML con tablas
            // Construir el mensaje en formato HTML con tablas
            var mensajeCotizacion = $@"
                <h1 style='text-align: center;'>Cotización Sal-Ticos</h1>
                <p><strong>Monto total:</strong> ₡{cotizacionActual.MontoTotal:N2}</p>
                <p><strong>Monto total con IVA:</strong> ₡{cotizacionActual.MontoTotalIVA:N2}</p>
                <hr>
                <h2>Resumen de cotización</h2>";

            if (cotizacionActual.AlimentacionSeleccionada != null && cotizacionActual.AlimentacionSeleccionada.Any())
            {
                mensajeCotizacion += @"
                <h3>Alimentación seleccionada:</h3>
                <table border='1' style='width: 100%; border-collapse: collapse; text-align: center;'>
                    <thead>
                        <tr>
                            <th>Nombre</th>
                            <th>Cantidad</th>
                            <th>Precio total</th>
                        </tr>
                    </thead>
                    <tbody>";
                foreach (var opcion in cotizacionActual.AlimentacionSeleccionada)
                {
                    mensajeCotizacion += $@"
                    <tr>
                        <td>{opcion.Alimentacion.Nombre}</td>
                        <td>{opcion.Cantidad}</td>
                        <td>₡{opcion.Alimentacion.Precio * opcion.Cantidad:N2}</td>
                    </tr>";
                }
                mensajeCotizacion += "</tbody></table>";
            }

            if (cotizacionActual.TarifaTransporteSeleccionada != null && cotizacionActual.TarifaTransporteSeleccionada.Any())
            {
                mensajeCotizacion += @"
                <h3>Transporte seleccionado:</h3>
                <table border='1' style='width: 100%; border-collapse: collapse; text-align: center;'>
                    <thead>
                        <tr>
                            <th>Provincia</th>
                            <th>Precio</th>
                        </tr>
                    </thead>
                    <tbody>";
                foreach (var tarifa in cotizacionActual.TarifaTransporteSeleccionada)
                {
                    mensajeCotizacion += $@"
                    <tr>
                        <td>{tarifa.Provincia}</td>
                        <td>₡{tarifa.Precio:N2}</td>
                    </tr>";
                }
                mensajeCotizacion += "</tbody></table>";
            }

            if (cotizacionActual.ServiciosSeleccionados != null && cotizacionActual.ServiciosSeleccionados.Any())
            {
                mensajeCotizacion += @"
                <h3>Servicios adicionales seleccionados:</h3>
                <table border='1' style='width: 100%; border-collapse: collapse; text-align: center;'>
                    <thead>
                        <tr>
                            <th>Cantidad</th>
                            <th>Servicio</th>
                            <th>Precio unitario</th>
                            <th>Precio total</th>
                        </tr>
                    </thead>
                    <tbody>";
                foreach (var servicio in cotizacionActual.ServiciosSeleccionados)
                {
                    mensajeCotizacion += $@"
                    <tr>
                        <td>{servicio.Cantidad}</td>
                        <td>{servicio.Servicio.Nombre}</td>
                        <td>₡{servicio.Servicio.Precio:N2}</td>
                        <td>₡{servicio.Servicio.Precio * servicio.Cantidad:N2}</td>
                    </tr>";
                }
                mensajeCotizacion += "</tbody></table>";
            }

            if (cotizacionActual.InflablesSeleccionados != null && cotizacionActual.InflablesSeleccionados.Any())
            {
                mensajeCotizacion += @"
                <h3>Inflables seleccionados:</h3>
                <table border='1' style='width: 100%; border-collapse: collapse; text-align: center;'>
                    <thead>
                        <tr>
                            <th>Nombre</th>
                            <th>Horas adicionales</th>
                            <th>Precio base</th>
                            <th>Precio por hora adicional</th>
                            <th>Precio total</th>
                        </tr>
                    </thead>
                    <tbody>";
                foreach (var inflable in cotizacionActual.InflablesSeleccionados)
                {
                    var precioTotal = inflable.Inflable.Precio + inflable.HorasAdicionales * inflable.Inflable.PrecioHoraAdicional;
                    mensajeCotizacion += $@"
                    <tr>
                        <td>{inflable.Inflable.Nombre}</td>
                        <td>{inflable.HorasAdicionales}</td>
                        <td>₡{inflable.Inflable.Precio:N2}</td>
                        <td>₡{inflable.Inflable.PrecioHoraAdicional:N2}</td>
                        <td>₡{precioTotal:N2}</td>
                    </tr>";
                }
                mensajeCotizacion += "</tbody></table>";
            }

            if (cotizacionActual.MobiliariosSeleccionados != null && cotizacionActual.MobiliariosSeleccionados.Any())
            {
                mensajeCotizacion += @"
            <h3>Mobiliarios seleccionados:</h3>
            <table border='1' style='width: 100%; border-collapse: collapse; text-align: center;'>
                <thead>
                    <tr>
                        <th>Nombre</th>
                        <th>Cantidad</th>
                        <th>Precio unitario</th>
                        <th>Precio total</th>
                    </tr>
                </thead>
                <tbody>";
                foreach (var mobiliario in cotizacionActual.MobiliariosSeleccionados)
                {
                    var precioTotal = mobiliario.Mobiliario.Precio * mobiliario.Cantidad;
                    mensajeCotizacion += $@"
                <tr>
                    <td>{mobiliario.Mobiliario.Nombre}</td>
                    <td>{mobiliario.Cantidad}</td>
                    <td>₡{mobiliario.Mobiliario.Precio:N2}</td>
                    <td>₡{precioTotal:N2}</td>
                </tr>";
                }
                mensajeCotizacion += "</tbody></table>";
            }

            mensajeCotizacion += $@"
            <hr>
            <p style='text-align: center;'>© Sal-Ticos</p>";



            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();

                    await emailSender.SendEmailAsync(
                        correo,
                        "Cotización",
                        mensajeCotizacion
                    );

                    


                }

            }
            catch (Exception ex)
            {
                // Manejo de errores (opcional)
                TempData["Error"] = "Error al enviar correo";
            }

            // No se redirige, simplemente retorna NoContent para que no haya navegación
            TempData["CotizacionActual"] = JsonConvert.SerializeObject(cotizacionActual);
            TempData["CotizacionActualCorreo"] = JsonConvert.SerializeObject(cotizacionActual);
            TempData.Keep("CotizacionActual");
            TempData.Keep("CotizacionActualCorreo");
            return NoContent(); ;
        }


        private void AddHeaderAndFooter(Document document, PdfWriter writer)
        {
            // Agregar encabezado
            var headerTable = new PdfPTable(2) { WidthPercentage = 100 };
            float[] columnWidths = { 1f, 3f }; 
            headerTable.SetWidths(columnWidths);

            
            var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imagenes", "logo.jpg");
            if (System.IO.File.Exists(logoPath))
            {
                var logo = Image.GetInstance(logoPath);
                logo.ScaleToFit(150f, 150f); 
                var logoCell = new PdfPCell(logo)
                {
                    Border = Rectangle.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                headerTable.AddCell(logoCell);
            }
            else
            {
                var emptyCell = new PdfPCell(new Phrase(" ")) { Border = Rectangle.NO_BORDER };
                headerTable.AddCell(emptyCell);
            }

            // Título del encabezado
            var baseFontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf");
            var baseFont = BaseFont.CreateFont(baseFontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            var headerFont = new Font(baseFont, 16, Font.BOLD);

            var companyName = new Phrase("", headerFont); 
            var companyCell = new PdfPCell(companyName)
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            headerTable.AddCell(companyCell);

            // Posicionar el encabezado manualmente en la parte superior
            headerTable.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
            headerTable.WriteSelectedRows(0, -1, document.LeftMargin, document.PageSize.Height - 10, writer.DirectContent);

            
            document.Add(new Paragraph(" "));

            // Agregar pie de página
            var footerTable = new PdfPTable(1) { WidthPercentage = 100 };
            var footerText = new Phrase("Página generada automáticamente - © Sal-ticos", FontFactory.GetFont(FontFactory.HELVETICA, 10, Font.ITALIC));
            var footerCell = new PdfPCell(footerText)
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_CENTER
            };
            footerTable.AddCell(footerCell);

            // Posicionar el pie de página manualmente en la parte inferior
            footerTable.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
            footerTable.WriteSelectedRows(0, -1, document.LeftMargin, document.BottomMargin, writer.DirectContent);
        }



    }
}
