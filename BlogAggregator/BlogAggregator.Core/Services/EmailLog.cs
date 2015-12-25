using System.Net;
using System.Net.Mail;

namespace BlogAggregator.Core.Services
{
    public class EmailLog
    {
        public void SendEmail(string to, string subject, string body)
        {
            var mm = new MailMessage
            {
                //From = new MailAddress("azure@origincodeacademy.com", "Origin Blog Errors"),
                From = new MailAddress("kdssnyderdev@gmail.com", "Origin Blog Errors"),
                Subject = subject
            };

            mm.To.Add(to);

            mm.Body = body;

            var client = new SmtpClient
            {
                Credentials = new NetworkCredential("soldityesterday@gmail.com", "gI6F_zbQVNZYQjXomr9s3g"),
                Port = 587,
                EnableSsl = true,
                Host = "smtp.mandrillapp.com"
            };

            client.Send(mm);
        }
    }
}
