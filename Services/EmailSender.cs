using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using System.Net;
using System.Net.Mail;

namespace Computer_Store.Services
{
 public class EmailSender : IEmailSender
 {
  public async Task SendEmailAsync(string email, string subject, string htmlMessage)
  {
   var frommail = "ELectronic_Shop_Store@outlook.com";
   var frompassword = "Youse.0155";
   var message = new MailMessage();
   message.From = new MailAddress(frommail);
   message.Subject = subject;
   message.To.Add(email);
   message.Body = $"<html><body>{htmlMessage}</body></html> ";
   message.IsBodyHtml = true;
   var smtpclient = new SmtpClient(host:"smtp-mail.outlook.com")
   {
    Port = 587,
    Credentials = new NetworkCredential(frommail, frompassword),
    EnableSsl = true
   };
   smtpclient.Send(message);
  }
 }
}
