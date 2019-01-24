using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Web;

namespace Scoreboards.Services
{
    public class EmailService : IEmailSender
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return SendEmailAsyncSandGridAsync(email, subject, htmlMessage);
        }

        private async Task SendEmailAsyncSandGridAsync(string email, string subject, string htmlMessage)
        {
            //region formatter
            string text = string.Format("Please click on this link to {0}: {1}", subject, htmlMessage);
            string html = htmlMessage;

            //html += HttpUtility.HtmlEncode(@"Or click on the copy the following link on the browser:" + message.Body);
            //endregion

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress("donotreply@theinstilleryscoreboards.com");
            msg.To.Add(new MailAddress(email));
            msg.Subject = subject;
            msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain));
            msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));

            SmtpClient smtpClient = new SmtpClient("smtp.sendgrid.net", Convert.ToInt32(587));
            NetworkCredential credentials = new NetworkCredential(
                _config["SendGridCredentials:SEND_GRID_USERNAME"],
                _config["SendGridCredentials:SEND_GRID_PASSWORD"]);
            smtpClient.Credentials = credentials;
            smtpClient.EnableSsl = true;
            smtpClient.Send(msg);

            await Task.CompletedTask;
        }
    }
}
