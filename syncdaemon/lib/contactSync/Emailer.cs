﻿using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Graph;

namespace syncdaemon
{
    public class Emailer
    {
        public static async void send(IConfigurationRoot config)
        {
            var list = getEmailAdds(config);
            var badList = checkForBad(list);
            string nameList = badList.Count() > 0 ? makeNameList(badList) : null;

            var client = new GraphClient().getClient(config);
            string sendTo = config.GetSection("sendToEmail").Value;
            var message = new Message
            {
                Subject = "SYSTEM GENERATED: Invalid Email Address",
                Body = new ItemBody
                {
                    ContentType = BodyType.Text,
                    Content = "Please check the Email Addresses for the following patients in Crystal: \r\n" + nameList
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
            if(nameList != null)
            {
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
            else
            {
                string log = "No invalid emails found";
                Logger.log("runLogger", log);
            }            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config">The IConfigurationRoot object to pass to the db context</param>
        /// <returns>List of string[]</returns>
        private static List<string[]> getEmailAdds(IConfigurationRoot config)
        {
            using (var db = new AccTestContext(config))
            {
                var list = db.Patient.Select(p => new[] { p.Email, p.FirstName, p.LastName } ).ToList();
                return list;
            }
        }
        /// <summary>
        /// This function handles validating the email addresses and returns a list of names to be sent in an email
        /// </summary>
        /// <param name="list">A list of string arrays that contain an EmailAddress, FirstName, and LastName</param>
        /// <returns>List of strings</returns>
        private static List<string> checkForBad(List<string[]> list)
        {
            var temp = new List<string>();
            foreach(var e in list)
            {
                if( e[0] != null && !validate(e[0]) )
                {
                    string name = e[1] + " " + e[2];
                    temp.Add(name);
                }
            }
            return temp;
        }

        //******************************** HELPER FUNCTIONS ******************

        /// <summary>
        /// Brings in a string.  Runs the two validator functions and returns a bool.
        /// </summary>
        /// <param name="s">A string that looks like an email address</param>
        /// <returns>Boolean</returns>
        private static bool validate(string s)
        {
            bool f = isFormatted(s) ? true : false;
            bool v = f ? isValidExtension(s) : false;
            return v;
        }
        ///<summary>
        ///Checks that an email address is in correct email format by attempting to create a System.Net.Mail.MailAddress object from the provided string.
        ///<example>name@domain.extension</example>
        ///</summary>
        ///<param name="email">A string that looks like an email address</param>
        ///<returns>Boolean</returns>
        private static bool isFormatted(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return true;
            }
            catch (Exception e)
            {
                Logger.log("error", e.Message);
                return false;
            }
        }
        ///<summary>
        ///Checks that a domain extension is valid according to IANA.org
        /// domain list can be found at https://data.iana.org/TLD/tlds-alpha-by-domain.txt 
        /// .txt file created 06/2020 from current domain list
        ///<example>.com, .net, .tech, etc.</example>
        ///</summary>
        ///<param name="email">A string email address</param>
        ///<returns>Boolean</returns>
        private static bool isValidExtension(string email)
        {
            string[] domexts = System.IO.File.ReadAllLines(@"../syncdaemon/lib/assets/validate.txt");
            string ext = getExtension(email);
            return domexts.Contains(ext) ? true : false;
        }
        ///<summary>
        ///Splits the email address on "." and grabs the last word in the array. This should be the extension.
        ///</summary>
        ///<param name="email">A string email address</param>
        ///<returns>String</returns>
        private static string getExtension(string email)
        {
            char splitter = '.';
            string[] temp = email.Split(splitter, StringSplitOptions.None);
            return temp[^1].ToString().ToUpper();
            //temp.Length - 1
        }
        private static string makeNameList(List<string> list)
        {
            string temp = "\r\n";
            foreach(string n in list)
            {
                temp += (n + "\r\n");
            }
            return temp;
        }
    }
}
