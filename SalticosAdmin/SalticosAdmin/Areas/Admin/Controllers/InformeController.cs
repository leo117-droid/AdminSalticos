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

        public IActionResult InformeInflables()
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

    }
}
