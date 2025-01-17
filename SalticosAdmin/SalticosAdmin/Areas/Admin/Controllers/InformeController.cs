using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using SalticosAdmin.Modelos;
using SalticosAdmin.Servicios;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;

namespace SalticosAdmin.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class InformeController : Controller
    {
        private readonly InformeServicio _informeServicio;

        public InformeController(InformeServicio informeServicio)
        {
            _informeServicio = informeServicio;
        }

        public IActionResult Informe()
        {
            // Obtener los datos de los inflables más solicitados
            var inflablesMasSolicitados = _informeServicio.ObtenerInflablesMasSolicitados();
            ViewBag.InflablesMasSolicitados = inflablesMasSolicitados;

            var alimentosMasSolicitados = _informeServicio.ObtenerAlimentosMasSolicitados();
            ViewBag.AlimentosMasSolicitados = alimentosMasSolicitados;

            var mobiliariosMasSolicitados = _informeServicio.ObtenerMobiliariosMasSolicitados();
            ViewBag.MobiliariosMasSolicitados = mobiliariosMasSolicitados;
            // Pasar los datos a la vista
            return View();
        }



        // Acción para generar el informe
        public IActionResult GenerarInformeInflables()
        {
            // Obtener inflables más solicitados con la cantidad de veces que fueron utilizados
            var inflablesMasSolicitados = _informeServicio.ObtenerInflablesMasSolicitados();

            // Crear el documento PDF
            using (var memoryStream = new MemoryStream())
            {
                var document = new Document(PageSize.A4);
                PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                // Título del informe
                // Crear el título del informe y centrarlo
                Paragraph titulo = new Paragraph("Informe de inflables más solicitados", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14));
                titulo.Alignment = Element.ALIGN_CENTER; // Centrar el texto
                document.Add(titulo);
                document.Add(new Paragraph(" "));

                document.Add(new Paragraph($"Fecha: {DateTime.Now.ToShortDateString()}"));
                document.Add(new Paragraph(" "));

                // Crear la tabla para mostrar los inflables
                PdfPTable table = new PdfPTable(2);
                table.AddCell("Nombre del Inflable");
                table.AddCell("Cantidad de veces solicitado");

                // Agregar los inflables a la tabla
                foreach (var inflable in inflablesMasSolicitados)
                {
                    table.AddCell(inflable.Nombre);
                    table.AddCell(inflable.Cantidad.ToString());
                }

                document.Add(table);
                document.Close();

                // Descargar el PDF
                return File(memoryStream.ToArray(), "application/pdf", "InformeInflablesSolicitados.pdf");
            }
        }

        public IActionResult GenerarInformeAlimentos()
        {
            var alimentosMasSolicitados = _informeServicio.ObtenerAlimentosMasSolicitados();

            using (var memoryStream = new MemoryStream())
            {
                var document = new Document(PageSize.A4);
                PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                // Título del informe
                // Crear el título del informe y centrarlo
                Paragraph titulo = new Paragraph("Informe de alimentación más solicitada", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14));
                titulo.Alignment = Element.ALIGN_CENTER; // Centrar el texto
                document.Add(titulo);
                document.Add(new Paragraph(" "));

                document.Add(new Paragraph($"Fecha: {DateTime.Now.ToShortDateString()}"));
                document.Add(new Paragraph(" "));

                // Crear la tabla para mostrar los alimentos
                PdfPTable table = new PdfPTable(2);
                table.AddCell("Nombre del Alimento");
                table.AddCell("Cantidad de veces solicitado");

                foreach (var alimento in alimentosMasSolicitados)
                {
                    table.AddCell(alimento.Nombre);
                    table.AddCell(alimento.Cantidad.ToString());
                }

                document.Add(table);
                document.Close();

                return File(memoryStream.ToArray(), "application/pdf", "InformeAlimentosSolicitados.pdf");
            }
        }

        public IActionResult GenerarInformeMobiliarios()
        {
            var mobiliariosMasSolicitados = _informeServicio.ObtenerMobiliariosMasSolicitados();

            using (var memoryStream = new MemoryStream())
            {
                var document = new Document(PageSize.A4);
                PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                // Título del informe
                // Crear el título del informe y centrarlo
                Paragraph titulo = new Paragraph("Informe de mobiliario más solicitado", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14));
                titulo.Alignment = Element.ALIGN_CENTER; // Centrar el texto
                document.Add(titulo);
                document.Add(new Paragraph(" "));

                document.Add(new Paragraph($"Fecha: {DateTime.Now.ToShortDateString()}"));
                document.Add(new Paragraph(" "));

                // Crear la tabla para mostrar los mobiliarios
                PdfPTable table = new PdfPTable(2);
                table.AddCell("Nombre del Mobiliario");
                table.AddCell("Cantidad de veces solicitado");

                foreach (var mobiliario in mobiliariosMasSolicitados)
                {
                    table.AddCell(mobiliario.Nombre);
                    table.AddCell(mobiliario.Cantidad.ToString());
                }

                document.Add(table);
                document.Close();

                return File(memoryStream.ToArray(), "application/pdf", "InformeMobiliariosSolicitados.pdf");
            }
        }

    }
}
