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
            var inflablesMasSolicitados = _informeServicio.ObtenerInflablesMasSolicitados();
            ViewBag.InflablesMasSolicitados = inflablesMasSolicitados;

            var alimentosMasSolicitados = _informeServicio.ObtenerAlimentosMasSolicitados();
            ViewBag.AlimentosMasSolicitados = alimentosMasSolicitados;

            var mobiliariosMasSolicitados = _informeServicio.ObtenerMobiliariosMasSolicitados();
            ViewBag.MobiliariosMasSolicitados = mobiliariosMasSolicitados;
            return View();
        }

        public IActionResult GenerarInformeInflables()
        {
            var inflablesMasSolicitados = _informeServicio.ObtenerInflablesMasSolicitados();

            using (var memoryStream = new MemoryStream())
            {
                var document = new Document(PageSize.A4, 36, 36, 54, 54);
                var writer = PdfWriter.GetInstance(document, memoryStream);
                document.Open();
                AddHeaderAndFooter(document, writer);

                var baseFontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf");
                var baseFont = BaseFont.CreateFont(baseFontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

                var titleFont = new Font(baseFont, 18, Font.BOLD);
                var sectionFont = new Font(baseFont, 14, Font.BOLD);
                var tableHeaderFont = new Font(baseFont, 12, Font.BOLD);
                var tableCellFont = new Font(baseFont, 12);



                document.Add(new Paragraph("Informe inflables  -  Sal-Ticos", titleFont) { Alignment = Element.ALIGN_CENTER });
                document.Add(new Paragraph(" "));


                var fechaParagraph = new Paragraph();
                fechaParagraph.Add(new Chunk("Fecha: ", new Font(baseFont, 12, Font.BOLD))); // Texto en negrita
                fechaParagraph.Add(new Chunk($"{DateTime.Now.ToShortDateString()}", new Font(baseFont, 12))); // Texto normal
                document.Add(fechaParagraph);

                document.Add(new Paragraph(" "));


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

                var table = CreateTable(2, new float[] { 2, 2});
                AddTableHeaders(table, new[] { "Nombre del inflable", "Cantidad de veces solicitado" });


                foreach (var inflable in inflablesMasSolicitados)
                {
                    AddCell(table, inflable.Nombre );
                    AddCell(table, inflable.Cantidad.ToString(), false);
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

                var document = new Document(PageSize.A4, 36, 36, 54, 54);
                var writer = PdfWriter.GetInstance(document, memoryStream);
                document.Open();
                AddHeaderAndFooter(document, writer);


                var baseFontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf");
                var baseFont = BaseFont.CreateFont(baseFontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

                var titleFont = new Font(baseFont, 18, Font.BOLD);
                var sectionFont = new Font(baseFont, 14, Font.BOLD);
                var tableHeaderFont = new Font(baseFont, 12, Font.BOLD);
                var tableCellFont = new Font(baseFont, 12);



                document.Add(new Paragraph("Informe alimentos  -  Sal-Ticos", titleFont) { Alignment = Element.ALIGN_CENTER });
                document.Add(new Paragraph(" "));


                var fechaParagraph = new Paragraph();
                fechaParagraph.Add(new Chunk("Fecha: ", new Font(baseFont, 12, Font.BOLD))); // Texto en negrita
                fechaParagraph.Add(new Chunk($"{DateTime.Now.ToShortDateString()}", new Font(baseFont, 12))); // Texto normal
                document.Add(fechaParagraph);

                document.Add(new Paragraph(" "));


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

                var table = CreateTable(2, new float[] { 2, 2 });
                AddTableHeaders(table, new[] { "Nombre del alimento", "Cantidad de veces solicitado" });

                foreach (var alimento in alimentosMasSolicitados)
                {
                    AddCell(table, alimento.Nombre);
                    AddCell(table, alimento.Cantidad.ToString(), false);
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
                var document = new Document(PageSize.A4, 36, 36, 54, 54);
                var writer = PdfWriter.GetInstance(document, memoryStream);
                document.Open();
                AddHeaderAndFooter(document, writer);


                var baseFontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf");
                var baseFont = BaseFont.CreateFont(baseFontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

                var titleFont = new Font(baseFont, 18, Font.BOLD);
                var sectionFont = new Font(baseFont, 14, Font.BOLD);
                var tableHeaderFont = new Font(baseFont, 12, Font.BOLD);
                var tableCellFont = new Font(baseFont, 12);



                document.Add(new Paragraph("Informe mobiliario  -  Sal-Ticos", titleFont) { Alignment = Element.ALIGN_CENTER });
                document.Add(new Paragraph(" "));


                var fechaParagraph = new Paragraph();
                fechaParagraph.Add(new Chunk("Fecha: ", new Font(baseFont, 12, Font.BOLD))); // Texto en negrita
                fechaParagraph.Add(new Chunk($"{DateTime.Now.ToShortDateString()}", new Font(baseFont, 12))); // Texto normal
                document.Add(fechaParagraph);

                document.Add(new Paragraph(" "));


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

                var table = CreateTable(2, new float[] { 2, 2 });
                AddTableHeaders(table, new[] { "Nombre del mobiliario", "Cantidad de veces solicitado" });


                foreach (var mobiliario in mobiliariosMasSolicitados)
                {
                    AddCell(table, mobiliario.Nombre);
                    AddCell(table, mobiliario.Cantidad.ToString(), false);
                }

                document.Add(table);
                document.Close();

                return File(memoryStream.ToArray(), "application/pdf", "InformeMobiliariosSolicitados.pdf");
            }
        }


        private void AddHeaderAndFooter(Document document, PdfWriter writer)
        {
            // Agregar encabezado
            var headerTable = new PdfPTable(2) { WidthPercentage = 100 };
            float[] columnWidths = { 1f, 3f };
            headerTable.SetWidths(columnWidths);


            var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imagenes", "UI","LOGO.jpeg");
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
