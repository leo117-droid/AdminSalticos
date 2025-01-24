using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SalticosAdmin.Modelos;
using SendGrid;
using SendGrid.Helpers.Mail;


namespace SalticosAdmin.Servicios
{
    public class EmailSender : IEmailSender
    {

        private readonly EmailSettings _emailSettings;

        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        //public async Task SendEmail(string subject, string toEmail, string username, string message, bool isHtml = false)
        //{
        //    var apiKey = _emailSettings.APIKey;
        //    var client = new SendGridClient(apiKey);
        //    var from = new EmailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName);
        //    var to = new EmailAddress(toEmail, username);

        //    var msg = MailHelper.CreateSingleEmail(from, to, subject, null, message);

        //    if (isHtml)
        //    {
        //        msg.HtmlContent = message;
        //    }
        //    else
        //    {
        //        msg.PlainTextContent = message;
        //    }

        //    var response = await client.SendEmailAsync(msg);
        //}

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SendGridClient(_emailSettings.APIKey);
            var from = new EmailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName);
            var to = new EmailAddress(email);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlMessage);
            await client.SendEmailAsync(msg);
        }




    }
}
