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
        private readonly GoogleOAuthService _googleOAuthService;

        public EmailService(IConfiguration configuration, GoogleOAuthService googleOAuthService)
        {
            _configuration = configuration;
            _googleOAuthService = googleOAuthService;
        }
        public async Task SendEmailAsync(string subject, string body, List<string> recipients)
        {
            var user = _configuration["SmtpSettings:User"];
            var accessToken = await _googleOAuthService.GetAccessTokenAsync();

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Encuestas UC", user));

            //foreach (var recipient in recipients)
            //{
            //    message.Bcc.Add(new MailboxAddress("", recipient));
            //}

            //BORRAR PARA PROD
            //-------------------------
            var correosPrueba = new List<string>
            {
                "oresteslacunza@gmail.com",
                "prappyacuruna08@continental.edu.pe"
            };

            foreach (var correo in correosPrueba)
            {
                message.To.Add(new MailboxAddress("", correo));
            }
            //-------------------------

            message.Subject = subject;
            message.Body = new TextPart("html") { Text = body };

            using var client = new SmtpClient();
            await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            var oauth2 = new SaslMechanismOAuth2(user, accessToken);
            await client.AuthenticateAsync(oauth2);

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
