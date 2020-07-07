using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Graph;

namespace syncdaemon
{
    public class Emailer
    {
        public async void send(IConfigurationRoot config, GraphServiceClient client)
        {
            string sendTo = config.GetSection("sendToEmail").Value;
            var message = new Message
            {
                Subject = "This is a test",
                Body = new ItemBody
                {
                    ContentType = BodyType.Text,
                    Content = "Hello from syncdaemon."
                },
                ToRecipients = new List<Recipient>()
                {
                    new Recipient
                    {
                        EmailAddress = new EmailAddress
                        {
                            Address = sendTo
                        }
                    }
                }
            };

            var saveToSentItems = false;
            var target = config.GetSection("targetUserId").GetChildren().Select(x => x.Value).ToList();
            try
            {
            await client.Users[target[^1]].SendMail(message, saveToSentItems)
                .Request()
                .PostAsync();

            Logger.log("runLogger", "email sent successfully");
            }
            catch(Exception e)
            {
                Logger.log("error", e.Message);
            }
            
        }
    }
}
