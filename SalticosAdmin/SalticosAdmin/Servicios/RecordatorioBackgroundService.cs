using SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;

public class RecordatorioBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RecordatorioBackgroundService> _logger;

    public RecordatorioBackgroundService(IServiceProvider serviceProvider, ILogger<RecordatorioBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Ejecutando la lógica de recordatorios...");

            using (var scope = _serviceProvider.CreateScope())
            {
                var unidadTrabajo = scope.ServiceProvider.GetRequiredService<IUnidadTrabajo>();
                var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();

                try
                {
                    // Obtener la fecha actual
                    var fechaActual = DateTime.Now.Date;

                    // Consultar los eventos con recordatorio para hoy
                    var recordatorios = (await unidadTrabajo.Evento
                        .ObtenerTodos(e => e.FechaRecordatorio.HasValue && e.FechaRecordatorio.Value.Date == fechaActual))
                        .ToList();

                    foreach (var evento in recordatorios)
                    {
                        try
                        {
                            // Crear el mensaje del recordatorio
                            string subject = $"Recordatorio para el evento del {evento.Fecha:dd/MM/yyyy}";
                            string message = $@"
                                <h1>Recordatorio de Evento</h1>
                                <p>Este es un recordatorio para su evento programado:</p>
                                <ul>
                                    <li><strong>Fecha:</strong> {evento.Fecha:dd/MM/yyyy}</li>
                                    <li><strong>Hora Inicio:</strong> {evento.HoraInicio}</li>
                                    <li><strong>Hora Final:</strong> {evento.HoraFinal}</li>
                                    <li><strong>Dirección:</strong> {evento.Direccion}</li>
                                    <li><strong>Provincia:</strong> {evento.Provincia}</li>
                                </ul>
                                <p>¡Gracias por confiar en nuestros servicios!</p>";

                            // Enviar el correo
                            await emailSender.SendEmailAsync(evento.Correo, subject, message);

                            _logger.LogInformation($"Recordatorio enviado a {evento.Correo} para el evento {evento.Id}");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Error al enviar el recordatorio para el evento {evento.Id}: {ex.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error al procesar los recordatorios: {ex.Message}");
                }
            }

            ////// Esperar un intervalo antes de la próxima ejecución (por ejemplo, 30 minutos)
            await Task.Delay(TimeSpan.FromMinutes(0.5), stoppingToken);
        }
    }
}

//public class RecordatorioBackgroundService : BackgroundService
//{
//    private readonly IServiceProvider _serviceProvider;
//    private readonly ILogger<RecordatorioBackgroundService> _logger;

//    public RecordatorioBackgroundService(IServiceProvider serviceProvider, ILogger<RecordatorioBackgroundService> logger)
//    {
//        _serviceProvider = serviceProvider;
//        _logger = logger;
//    }

//    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//    {
//        while (!stoppingToken.IsCancellationRequested)
//        {
//            _logger.LogInformation("Ejecutando la lógica de recordatorios...");

//            using (var scope = _serviceProvider.CreateScope())
//            {
//                var unidadTrabajo = scope.ServiceProvider.GetRequiredService<IUnidadTrabajo>();

//                try
//                {
//                    // Obtener la fecha actual
//                    var fechaActual = DateTime.Now.Date;

//                    // Consultar los recordatorios con fecha igual a hoy
//                    var recordatorios = (await unidadTrabajo.Evento
//                        .ObtenerTodos(r => r.FechaRecordatorio.HasValue && r.FechaRecordatorio.Value.Date == fechaActual))
//                        .ToList();

//                    foreach (var recordatorio in recordatorios)
//                    {
//                        // Lógica para enviar el recordatorio (puede ser un correo, notificación, etc.)
//                        _logger.LogInformation($"Enviando recordatorio: {recordatorio}");
//                        // Ejemplo: await EnviarNotificacionAsync(recordatorio);
//                    }
//                }
//                catch (Exception ex)
//                {
//                    _logger.LogError($"Error al procesar los recordatorios: {ex.Message}");
//                }
//            }

//            // Esperar un intervalo antes de la próxima ejecución (por ejemplo, 1 hora)
//            await Task.Delay(TimeSpan.FromMinutes(0.5), stoppingToken);
//        }
//    }

//}
