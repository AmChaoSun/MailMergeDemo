using MailMergeDemo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace MailMergeDemo.Helper
{
    public static class MailerHelper
    {
        private static readonly string defaultSmtpServer = Startup.StaticConfig["Email:SmtpServer"];
        private static readonly string defaultSmtpUsername = Startup.StaticConfig["Email:SmtpUsername"];
        private static readonly string defaultSmtpPassword = Startup.StaticConfig["Email:SmtpPassword"];
        private static readonly string defaultFromAddress = Startup.StaticConfig["Email:FromAddress"];
        private static readonly int defaultPort = int.Parse(Startup.StaticConfig["Email:SmtpPort"]);
        private static readonly bool defaultUseDefaultCredentials = bool.Parse(Startup.StaticConfig["Email:UseDefaultCredentials"]);
        private static readonly bool defaultEnableSsl = bool.Parse(Startup.StaticConfig["Email:EnableSsl"]);

        public static async Task SendBulkEmailsAsync(EmailTemplate template, IFormFile paramsheet)
        {
            var sheetContent = await ResolveFileAsync(paramsheet);

            //recipient param is essential for sending emails
            int recipientIndex = sheetContent.Headers.findIndex("{{recipient}}");
            if (recipientIndex == -1)
            {
                return;
            }

            using(var client = CreateDefaultSmtpClient())
            {
                foreach (var entry in sheetContent.Entries)
                {
                    string to = entry[recipientIndex];
                    string subject = template.Title;
                    string body = template.Body;
                    for (int i = 0; i < sheetContent.Headers.Length; i++)
                    {
                        body = body.Replace(sheetContent.Headers[i], entry[i]);
                        subject = subject.Replace(sheetContent.Headers[i], entry[i]);
                    }
                    var mail = GenerateMail(to, body, subject);
                    client.Send(mail);
                }
            }
            
        }

        private static MailMessage GenerateMail(string to, string body, string subject)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(defaultFromAddress);
            mailMessage.To.Add(to);
            mailMessage.Body = body;
            mailMessage.Subject = subject;
            return mailMessage;
        }
        private static SmtpClient CreateDefaultSmtpClient()
        {
            return CreateSmtpClient(defaultSmtpServer, defaultPort, defaultSmtpUsername, defaultSmtpPassword, defaultUseDefaultCredentials, defaultEnableSsl);
        }
        private static SmtpClient CreateSmtpClient(string smtpServer, int port, string smtpUsername, string smtpPassword, bool useDefaultCredentials, bool enableSsl)
        {
            //SmtpClient client = new SmtpClient(smtpServer);
            //client.UseDefaultCredentials = useDefaultCredentials;
            //client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
            //return client;
            return new SmtpClient
            {
                Host = smtpServer,
                Port = port,
                EnableSsl = enableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = useDefaultCredentials,
                Credentials = new NetworkCredential(smtpUsername, smtpPassword)
            };
        }

        private static async Task<CSVContent> ResolveFileAsync(IFormFile file)
        {
            var result = new StringBuilder();
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                while (reader.Peek() >= 0)
                    result.AppendLine(await reader.ReadLineAsync());
            }
            var content = result.ToString().Split("\r\n");

            //At least header line, param line and last empty line
            if (content.Length < 3)
            {
                return null;
            }
            Array.Resize(ref content, content.Length - 1);
            return new CSVContent { Headers = content[0].Split(","), Entries = content.Skip(1).Select(item => item.Split(",")) };
        }

        private class CSVContent
        {
            public string[] Headers { get; set; }
            public IEnumerable<string[]> Entries { get; set; }
        }
    }

    public static class Extensions
    {
        public static int findIndex<T>(this T[] array, T item)
        {
            return Array.FindIndex(array, val => val.Equals(item));
        }
    }
}
