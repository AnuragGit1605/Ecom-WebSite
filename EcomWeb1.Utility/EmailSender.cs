using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace EcomWeb1.Utility
{
    public class EmailSender : IEmailSender
    {
        private EmailSettings _emailSettings { get; }
        public EmailSender( IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

       public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            //call the email methods
            Execute(email, subject, htmlMessage).Wait();
            return Task.FromResult(0);
        }
        //method for email
        public async Task Execute(string email, string subject, string htmlMessage)
        {
            try
            {
                string toEmail = string.IsNullOrEmpty(email) ? _emailSettings.ToEmail : email;
                MailMessage mailMessage = new MailMessage()
                {
                    From=new MailAddress(_emailSettings.UsernameEmail,"My Email Address"),
                };
                mailMessage.To.Add(toEmail);
                mailMessage.CC.Add(_emailSettings.CcEmail);
                mailMessage.Bcc.Add(_emailSettings.BccEmail);
                mailMessage.Subject = "ShoppingApp:" + subject;
                mailMessage.Body = htmlMessage;
                mailMessage.IsBodyHtml = true;
                mailMessage.Priority = MailPriority.High;
                using(SmtpClient smtp=new SmtpClient(_emailSettings.PrimaryDomain,_emailSettings.PrimaryPort))
                {
                    smtp.Credentials = new NetworkCredential(_emailSettings.UsernameEmail, _emailSettings.UsernamePassword);
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(mailMessage);                        
                }
            }
            catch(Exception ex)
            {
                string str = ex.Message;
            }
        }
    }
}
