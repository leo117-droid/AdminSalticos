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
                document.Add(new Paragraph("Informe de Inflables Más Solicitados"));
                document.Add(new Paragraph($"Fecha: {DateTime.Now.ToShortDateString()}"));
                document.Add(new Paragraph(" "));

                // Crear la tabla para mostrar los inflables
                PdfPTable table = new PdfPTable(2);
                table.AddCell("Nombre del Inflable");
                table.AddCell("Cantidad de Veces Solicitado");

                // Agregar los inflables a la tabla
                foreach (var inflable in inflablesMasSolicitados)
                {
                    table.AddCell(inflable.Nombre);
                    table.AddCell(inflable.Cantidad.ToString());
                }

                document.Add(table);
                document.Close();

                // Descargar el PDF
                return File(memoryStream.ToArray(), "application/pdf", "Informe_Inflables_Solicitados.pdf");
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
                document.Add(new Paragraph("Informe de Alimentos Más Solicitados"));
                document.Add(new Paragraph($"Fecha: {DateTime.Now.ToShortDateString()}"));
                document.Add(new Paragraph(" "));

                // Crear la tabla para mostrar los alimentos
                PdfPTable table = new PdfPTable(2);
                table.AddCell("Nombre del Alimento");
                table.AddCell("Cantidad de Veces Solicitado");

                foreach (var alimento in alimentosMasSolicitados)
                {
                    table.AddCell(alimento.Nombre);
                    table.AddCell(alimento.Cantidad.ToString());
                }

                document.Add(table);
                document.Close();

                return File(memoryStream.ToArray(), "application/pdf", "Informe_Alimentos_Solicitados.pdf");
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
                document.Add(new Paragraph("Informe de Mobiliarios Más Solicitados"));
                document.Add(new Paragraph($"Fecha: {DateTime.Now.ToShortDateString()}"));
                document.Add(new Paragraph(" "));

                // Crear la tabla para mostrar los mobiliarios
                PdfPTable table = new PdfPTable(2);
                table.AddCell("Nombre del Mobiliario");
                table.AddCell("Cantidad de Veces Solicitado");

                foreach (var mobiliario in mobiliariosMasSolicitados)
                {
                    table.AddCell(mobiliario.Nombre);
                    table.AddCell(mobiliario.Cantidad.ToString());
                }

                document.Add(table);
                document.Close();

                return File(memoryStream.ToArray(), "application/pdf", "Informe_Mobiliarios_Solicitados.pdf");
            }
        }

    }
}
