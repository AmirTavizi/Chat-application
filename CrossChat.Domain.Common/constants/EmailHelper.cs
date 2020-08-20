using Mailjet.Client;
using Mailjet.Client.Resources;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CrossChat.Domain.Common.constants
{
    public class EmailHelper
    {
        public static string ApiKey;
        public static string ApiSecret;
        public static string Subject;
        public static string Body;

        public static async Task<bool> SendMailAsync(string to, string name, string url)
        {

            MailjetClient client = new MailjetClient(ApiKey, ApiSecret)
            {
                Version = ApiVersion.V3_1,
            };

            MailjetRequest request = new MailjetRequest
            {
                Resource = Send.Resource,
            };
            request.Property(Send.Messages, 
                new JArray {
                     new JObject 
                     {
                         {
                               "From",
                               new JObject 
                               {
                                   {"Email", "amir.tavizi@gmail.com"},
                                   {"Name", "Cross - Reset Password"}
                               }
                         }, 
                         {
                               "To",
                               new JArray 
                               {
                                    new JObject 
                                    {
                                         {"Email",to}, 
                                         {"Name",name}
                                    }
                               }
                        }, 
                        {"Subject",Subject}, 
                        {"TextPart",""}, 
                        {"HTMLPart", Body.Replace("[url]",url)}, 
                        {"CustomID","AppGettingStartedTest"}
                     }
             });
            MailjetResponse response = await client.PostAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

       
    }
}
