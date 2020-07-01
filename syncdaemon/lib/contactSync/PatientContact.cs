using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Graph;

namespace syncdaemon
{
    class PatientContact
    {
        ///<summary>
        ///This is the method for processing the SQL Patient object into a Microsoft Graph Contact object
        ///</summary>
        ///<param name="patient">This is the object from the SQL query</param>
        ///<returns>A Microsft Graph Contact object</returns>
        public Contact NewContact(Patient patient)
        {
            var newContact = new Contact()
            {
                FileAs = patient.AccountId.ToString(),
                GivenName = patient.FirstName,
                Surname = patient.LastName,
                MiddleName = patient.MiddleName,
                NickName = patient.Nickname,
                BusinessPhones = new List<String>()
                {
                    patient.WorkPhone
                },
                HomePhones = new List<String>()
                {
                    patient.HomePhone
                },
                MobilePhone = patient.CellPhone,
                HomeAddress = new PhysicalAddress()
                {
                    City = patient.City,
                    PostalCode = patient.Zipcode.ToString(),
                    State = patient.State
                },

                Birthday = new DateTimeOffset(DateTime.Parse(patient.Dob.ToString()))
            };
            newContact.HomeAddress.Street = patient.Address2 != null
                    ? (patient.Address1 + " " + patient.Address2)
                    : patient.Address1;
            if (patient.Email != null)
            {
                //DONE: validate format and domain extension of email
                bool f = isFormatted(patient.Email) ? true : false;
                bool v = f ? isValidExtension(patient.Email) : false;
                newContact.EmailAddresses = v ? newEmail(patient.Email, (patient.FirstName + " " + patient.LastName)) : null;
                if (!v) Logger.log("errorLog", ("Bad extension " + patient.Email));
            };
            return newContact;
        }


        //******************************** HELPER METHODS ******************
        ///<summary>
        ///Checks that an email address is in correct email format by attempting to create a System.Net.Mail.MailAddress object from the proveded string.
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
                Logger.log("errorLog", (e.Message + " " + email));
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
            string[] domexts = System.IO.File.ReadAllLines(@"lib\validate.txt"); //get relative path
            string ext = getExtension(email);
            return domexts.Contains(ext) ? true : false;
        }
        ///<summary>
        ///Splits the email adress on "." and grabs the last word in the array. This should be the exentsion.
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
        ///<summary>
        ///Brings in a Name and Email Address as strings and creates a Microsoft Graph Email object
        ///</summary>
        ///<param name="email">A string email address</param>
        ///<param name="name">A string name</param>
        ///<returns>MS Conctact EmailAddress object</returns>
        private static List<EmailAddress> newEmail(string email, string name)
        {
            List<EmailAddress> temp = new List<EmailAddress>()
            {
                new EmailAddress
                {
                    Address = email,
                    Name = name
                }
            };
            return temp;
        }
    }
}
