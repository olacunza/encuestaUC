using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssesmentUC.Model.Entity;
using AssesmentUC.Service.Service.Interface;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;


namespace AssesmentUC.Service.Service.Impl
{
    public class EmailService: IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendEmailAsync(string subject, string body, List<string> recipients)
        {
            var host = _configuration["SmtpSettings:Host"];
            var port = int.Parse(_configuration["SmtpSettings:Port"] ?? "587");
            var user = _configuration["SmtpSettings:User"];
            var password = _configuration["SmtpSettings:Password"];

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Encuestas UC", user));

            //foreach (var recipient in recipients)
            //{
            //    message.To.Add(new MailboxAddress("", recipient));
            //}

            message.To.Add(new MailboxAddress("", "oresteslacunza@gmail.com"));     //BORRAR PARA PROD

            message.Subject = subject;
            message.Body = new TextPart("plain") { Text = body };

            using var client = new SmtpClient();
            await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(user, password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
