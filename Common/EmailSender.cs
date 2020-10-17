using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Newtonsoft.Json.Linq;

namespace Common
{
    public class EmailSender
    {
        public static async Task<bool> SendEmailAsync(string receiverEmail, string subject, string body)
        {
            MailjetClient mjClient = new MailjetClient(KeyVault.MAIL_JET_PUBLIC_API_KEY, KeyVault.MAIL_JET_PRIVATE_API_KEY)
            {
                Version = ApiVersion.V3_1
            };

            MailjetRequest mjRequest = new MailjetRequest()
            {
                Resource = Send.Resource
            }.Property(Send.Messages, new JArray {
                new JObject {
                    {"From", new JObject {
                        {"Email", KeyVault.MAIL_JET_EMAIL},
                        {"Name", "no-reply"}
                    }},
                    {"To", new JArray {
                        new JObject {
                            {"Email", receiverEmail},
                            {"Name", "You"}
                        }
                    }},
                    {"Subject", subject},
                    {"HTMLPart", body}
                }
            });

            MailjetResponse response = await mjClient.PostAsync(mjRequest);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                throw new Exception(response.GetErrorMessage());
            }
        }
    }
}
