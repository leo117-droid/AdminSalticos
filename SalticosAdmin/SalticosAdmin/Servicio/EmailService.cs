using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SalticosAdmin.Modelos;
using SendGrid;
using SendGrid.Helpers.Mail;


namespace SalticosAdmin.Utilidades
{
    public class EmailService
    {

        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _settings = emailSettings.Value;
        }

        public async Task SendEmail(string subject, string toEmail, string username, string message, bool isHtml = false)
        {
            var apiKey = _settings.APIKey;
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(_settings.SenderEmail, _settings.SenderName);
            var to = new EmailAddress(toEmail, username);

            var msg = MailHelper.CreateSingleEmail(from, to, subject, null, message);

            if (isHtml)
            {
                msg.HtmlContent = message;
            }
            else
            {
                msg.PlainTextContent = message;
            }

            var response = await client.SendEmailAsync(msg);
        }
        public async Task SendEmailWithAttachment(string subject, string toEmail, string username, string message, byte[] fileBytes, string fileName, bool isHtml = false)
        {
            var apiKey = _settings.APIKey;
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(_settings.SenderEmail, _settings.SenderName);
            var to = new EmailAddress(toEmail, username);

            var msg = MailHelper.CreateSingleEmail(from, to, subject, null, message);

            if (isHtml)
            {
                msg.HtmlContent = message;
            }
            else
            {
                msg.PlainTextContent = message;
            }

            // Adjuntar el archivo al correo electrónico
            var file = Convert.ToBase64String(fileBytes);
            msg.AddAttachment(fileName, file, "application/pdf");

            var response = await client.SendEmailAsync(msg);
        }



    }
}
