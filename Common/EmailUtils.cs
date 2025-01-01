using System.Net.Mail;
using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using Microsoft.Extensions.Configuration;

namespace Common
{
    public class EmailUtils
    {
        private readonly IConfiguration _configuration;

        public EmailUtils(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public bool AccountConfirmationMail(string Username, string Password, string callbackUrl)
        {
            var _email = new MimeMessage();
            _email.From.Add(MailboxAddress.Parse("ak4559001@gmail.com"));
            _email.To.Add(MailboxAddress.Parse(Username));
            _email.Subject = "Account Verification Mail";
            _email.Body = new TextPart(TextFormat.Html)
            {
                Text = $"<p><Hi,</p>" +
            $"<p> Please verify your accout. Your password is :<span style='font-weight:bold'>" + Password + "</span></p>" +
            $"<p style='font-weight:bold'> {callbackUrl} <p>"
            };

            //Connect to the SMTP server For mail sending
            using var smtp = new SmtpClient();
            var Server = _configuration["MailCreds:Server"];
            var Port = _configuration["MailCreds:Port"];
            var SenderEmail = _configuration["MailCreds:SenderEmail"];
            var MailPassword = _configuration["MailCreds:Password"];

            smtp.Connect(Server, Convert.ToInt32(Port), SecureSocketOptions.StartTls);
            smtp.Authenticate(SenderEmail, MailPassword);
            smtp.Send(_email);
            return true;
        }

        public bool SendMail(string sendFrom,string sendTo,string body)
        {
            try
            {
                var _email = new MimeMessage();
                _email.From.Add(MailboxAddress.Parse(sendFrom));
                _email.To.Add(MailboxAddress.Parse(sendTo));
                _email.Subject = "VIR request is cancelled due to Duplicates in system.";
                _email.Body = new TextPart(TextFormat.Html)
                {
                    Text = body
                };

                //Connect to the SMTP server For mail sending
                using var smtp = new SmtpClient();
                var Server = _configuration["MailCreds:Server"];
                var Port = _configuration["MailCreds:Port"];
                var SenderEmail = _configuration["MailCreds:SenderEmail"];
                var MailPassword = _configuration["MailCreds:Password"];

                smtp.Connect(Server, Convert.ToInt32(Port), SecureSocketOptions.StartTls);
                smtp.Authenticate(SenderEmail, MailPassword);
                smtp.Send(_email);
                return true;
            }
            catch(Exception ex)
            {
               var error =  ex.Message;
                return false;
            }
        }
    }
}
