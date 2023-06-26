using System.Net;
using System.Net.Mail;

namespace ASP_201.Services.Email
{
    public class GmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        public GmailService(IConfiguration configuration)   // Через інжекційний конструктор
        {
            _configuration = configuration;
        }

        public bool Send(String mailTemplate, object model)
        {
            //
            String? template = null;
            String[] filenames = new String[]
            {
                mailTemplate,
                mailTemplate + ".html",
                "Services/Email/" + mailTemplate,
                "Services/Email/" + mailTemplate + ".html"
            };
            foreach (String filename in filenames)
            {
                if (System.IO.File.Exists(filename))
                {
                    template = System.IO.File.ReadAllText(filename);
                    break;
                }
            }
            if(template is null)
            {
                throw new ArgumentException($"Template '{mailTemplate}' not found");
            }
            // перевіряємо поштову конфігурацію
            String? host = _configuration["Smtp:Gmail:Host"];
            if (host is null) { throw new MissingFieldException("Missing configuration 'Smtp:Gmail:Host'"); }
            String? mailbox = _configuration["Smtp:Gmail:Email"];
            if (mailbox is null) { throw new MissingFieldException("Missing configuration 'Smtp:Gmail:Email'"); }
            String? password = _configuration["Smtp:Gmail:Password"];
            if (password is null) { throw new MissingFieldException("Missing configuration 'Smtp:Gmail:Password'"); }

            int port;
            try { port = Convert.ToInt32(_configuration["Smtp:Gmail:Port"]); }
            catch (Exception) { throw new MissingFieldException("Missing or invalid configuration 'Smtp:Gmail:Port'"); }
            bool Ssl;
            try { Ssl = Convert.ToBoolean(_configuration["Smtp:Gmail:Ssl"]); }
            catch (Exception) { throw new MissingFieldException("Missing or invalid configuration 'Smtp:Gmail:Ssl'"); }

            // Заповнюємо шаблон - проходимо по властивостях моделі та замінюємо
            // їх значення у шаблоні за збігом імен
            String? userEmail = null;
            foreach (var prop in model.GetType().GetProperties())
            {
                if (prop.Name == "Email") userEmail = prop.GetValue(model)?.ToString();
                String placeholder = $"{{{{{prop.Name}}}}}";
                if (template.Contains(placeholder))
                {
                    template = template.Replace(placeholder, prop.GetValue(model)?.ToString());
                }
            }
            if(userEmail is null)
            {
                throw new ArgumentException("No 'Email' property in model");
            }
            // TODO : перевірити залишок {{\w+}} плейсхолдерів у шаблоні
            using SmtpClient smtp = new(host, port)
            {
                EnableSsl = Ssl,
                Credentials = new NetworkCredential(mailbox, password)
            };
            MailMessage mailMessage = new()
            {
                From = new MailAddress(mailbox),
                Subject = "ASP-201 Project",
                IsBodyHtml = true,
                Body = template
            };
            mailMessage.To.Add(userEmail);
            try
            {
                smtp.Send(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                //_logger.LogWarning("Send Email exception");
                Console.WriteLine(ex.ToString());
                return true;
            }
        }
    }
}
