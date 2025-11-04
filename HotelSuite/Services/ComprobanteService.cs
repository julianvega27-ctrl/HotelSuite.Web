using HotelSuite.Application.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace HotelSuite.Services;

public class ComprobanteService
{
    public byte[] GenerarComprobantePDF(PagoDTO pago, ReservaDTO reserva, HuespedDTO huesped, HabitacionDTO habitacion)
    {
     // Configurar licencia (Community para uso no comercial)
        QuestPDF.Settings.License = LicenseType.Community;

    var document = Document.Create(container =>
        {
            container.Page(page =>
    {
    page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
          page.PageColor(Colors.White);
         page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

          // Encabezado
    page.Header().Element(ComposeHeader);

     // Contenido
    page.Content().Element(content => ComposeContent(content, pago, reserva, huesped, habitacion));

// Pie de página
    page.Footer().AlignCenter().Text(text =>
   {
           text.Span("Página ");
text.CurrentPageNumber();
        text.Span(" de ");
              text.TotalPages();
        text.Line($" - Generado el {DateTime.Now:dd/MM/yyyy HH:mm}");
   });
          });
        });

      return document.GeneratePdf();
    }

    private void ComposeHeader(IContainer container)
    {
    container.Row(row =>
      {
    row.RelativeItem().Column(column =>
       {
      column.Item().Text("HOTELSUITE").FontSize(24).Bold().FontColor(Colors.Blue.Darken2);
    column.Item().Text("Sistema de Gestión Hotelera").FontSize(12).FontColor(Colors.Grey.Darken1);
    column.Item().Text("RFC: HOTEL123456789").FontSize(10);
   column.Item().Text("Tel: (555) 123-4567").FontSize(10);
      });

            row.RelativeItem().AlignRight().Column(column =>
 {
  column.Item().AlignRight().Text("COMPROBANTE DE PAGO").FontSize(18).Bold();
     column.Item().AlignRight().Text($"Folio: {DateTime.Now:yyyyMMdd}-{new Random().Next(1000, 9999)}").FontSize(11);
    column.Item().AlignRight().Text($"Fecha: {DateTime.Now:dd/MM/yyyy}").FontSize(11);
           column.Item().AlignRight().Text($"Hora: {DateTime.Now:HH:mm}").FontSize(11);
   });
        });

     container.PaddingVertical(10).LineHorizontal(2).LineColor(Colors.Blue.Darken2);
    }

    private void ComposeContent(IContainer container, PagoDTO pago, ReservaDTO reserva, HuespedDTO huesped, HabitacionDTO habitacion)
    {
        container.PaddingVertical(20).Column(column =>
     {
            // Información del Huésped
            column.Item().Background(Colors.Grey.Lighten3).Padding(10).Column(section =>
            {
       section.Item().Text("INFORMACIÓN DEL HUÉSPED").FontSize(14).Bold();
   section.Item().PaddingTop(5).Row(row =>
      {
     row.RelativeItem().Text($"Nombre: {huesped.NombreCompleto}");
   row.RelativeItem().Text($"Email: {huesped.Email}");
     });
      section.Item().Row(row =>
    {
            row.RelativeItem().Text($"Teléfono: {huesped.Telefono}");
           row.RelativeItem().Text($"Documento: {huesped.DocumentoIdentidad}");
         });
        });

     column.Item().PaddingTop(20);

            // Información de la Reserva
    column.Item().Background(Colors.Blue.Lighten4).Padding(10).Column(section =>
    {
      section.Item().Text("DETALLES DE LA RESERVA").FontSize(14).Bold();
           section.Item().PaddingTop(5).Row(row =>
                {
        row.RelativeItem().Text($"ID Reserva: #{reserva.Id}");
    row.RelativeItem().Text($"Estado: {reserva.Estado}");
    });
       section.Item().Row(row =>
        {
          row.RelativeItem().Text($"Check-in: {reserva.FechaEntrada:dd/MM/yyyy}");
   row.RelativeItem().Text($"Check-out: {reserva.FechaSalida:dd/MM/yyyy}");
                });
     section.Item().Row(row =>
       {
     row.RelativeItem().Text($"Habitación: #{habitacion.Numero} - {habitacion.Tipo}");
            row.RelativeItem().Text($"Noches: {reserva.DiasEstancia}");
       });
            });

      column.Item().PaddingTop(20);

    // Desglose de Pagos
         column.Item().Text("DESGLOSE DE PAGOS").FontSize(14).Bold();
            column.Item().PaddingTop(10).Table(table =>
          {
                table.ColumnsDefinition(columns =>
   {
   columns.RelativeColumn(3);
       columns.RelativeColumn(2);
        columns.RelativeColumn(1);
  columns.RelativeColumn(2);
        });

             // Encabezado
           table.Header(header =>
      {
 header.Cell().Background(Colors.Blue.Darken2).Padding(5).Text("Concepto").FontColor(Colors.White).Bold();
    header.Cell().Background(Colors.Blue.Darken2).Padding(5).Text("Precio/Noche").FontColor(Colors.White).Bold();
       header.Cell().Background(Colors.Blue.Darken2).Padding(5).Text("Noches").FontColor(Colors.White).Bold();
       header.Cell().Background(Colors.Blue.Darken2).Padding(5).AlignRight().Text("Subtotal").FontColor(Colors.White).Bold();
     });

   // Fila de datos
        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text($"Habitación {habitacion.Tipo}");
           table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(habitacion.PrecioPorNoche.ToString("C"));
            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(reserva.DiasEstancia.ToString());
   table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignRight().Text(pago.Monto.ToString("C"));
    });

       column.Item().PaddingTop(20);

      // Información del Pago
          column.Item().Background(Colors.Green.Lighten4).Padding(10).Column(section =>
   {
   section.Item().Text("INFORMACIÓN DEL PAGO").FontSize(14).Bold();
    section.Item().PaddingTop(5).Row(row =>
         {
 row.RelativeItem().Text($"Método de Pago: {pago.Metodo}");
          row.RelativeItem().Text($"Fecha de Pago: {pago.FechaPago:dd/MM/yyyy HH:mm}");
                });
           section.Item().Row(row =>
                {
    row.RelativeItem().Text($"ID Pago: #{pago.Id}");
  });
            });

          column.Item().PaddingTop(20);

            // Total
            column.Item().AlignRight().Row(row =>
            {
           row.RelativeItem(3);
         row.RelativeItem().Background(Colors.Green.Darken2).Padding(10).Column(total =>
      {
                 total.Item().Text("TOTAL PAGADO").FontColor(Colors.White).FontSize(12).Bold();
        total.Item().Text(pago.Monto.ToString("C")).FontColor(Colors.White).FontSize(20).Bold();
         });
            });

   column.Item().PaddingTop(30);

  // Nota legal
   column.Item().Border(1).BorderColor(Colors.Grey.Lighten1).Padding(10).Column(nota =>
{
              nota.Item().Text("NOTAS IMPORTANTES").FontSize(10).Bold();
     nota.Item().PaddingTop(5).Text("• Este comprobante es válido como prueba de pago.").FontSize(9);
   nota.Item().Text("• Conserve este documento para cualquier aclaración.").FontSize(9);
     nota.Item().Text("• Para cualquier duda o aclaración, comuníquese al (555) 123-4567.").FontSize(9);
         nota.Item().Text("• No se aceptan devoluciones después de 24 horas del check-in.").FontSize(9);
   });

    column.Item().PaddingTop(20);

// Firma
            column.Item().Row(row =>
        {
    row.RelativeItem().AlignCenter().Column(firma =>
        {
    firma.Item().PaddingTop(30).LineHorizontal(1).LineColor(Colors.Black);
       firma.Item().PaddingTop(5).Text("Firma del Cliente").FontSize(10);
       });

                row.RelativeItem(2);

        row.RelativeItem().AlignCenter().Column(firma =>
       {
       firma.Item().PaddingTop(30).LineHorizontal(1).LineColor(Colors.Black);
             firma.Item().PaddingTop(5).Text("Firma Autorizada").FontSize(10);
  });
      });
        });
    }
}
