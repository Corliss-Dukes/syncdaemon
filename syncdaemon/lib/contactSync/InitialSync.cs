using System;
using Microsoft.Graph;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace syncdaemon
{
    public class InitialSync
    {
        ///<summary>
        /// This is the initial method. This should only be run once, to fill all User Contacts folders with the items from the db.
        ///</summary>
        ///<param name="config">The configuration object</param>
        public void RunInit(IConfigurationRoot config)
        {
            var plist = fetchFromSql(config);
            if (plist.Count() > 0)
            {
                var clist = processFromSql(plist);
                initialFill(clist, config);
            }
        }

        ///<summary>
        ///This function makes a sql call and grabs any Patients that have been edited in the last 24hrs
        ///</summary>
        ///<param name="config">The configuration object</param>
        ///<returns>List{Patient}</returns>
        static List<Patient> fetchFromSql(IConfigurationRoot config)
        {
            using (var db = new AccTestContext(config))
            {
                var list = db.Patient.Select(p => p).ToList();
                Logger.log("changeLog", list.Count().ToString());
                return list;
            }
        }

        ///<summary>
        ///This function brings in a List of Patient objects from SQL and processes them into MS Contact objects
        ///</summary>
        ///<param name="plist">The list of Patients to be processed</param>
        ///<returns>List{Contact}</returns>
        static List<Contact> processFromSql(List<Patient> plist)
        {
            var list = new List<Contact>();
            foreach (var p in plist)
            {
                list.Add(new PatientContact().NewContact(p));
            }
            return list;
        }

        ///<summary>
        /// This function should not be used in production. This will read the db, and add all contacts to MS Contacts
        ///</summary>
        static async void initialFill(List<Contact> list, IConfigurationRoot config)
        {
            var target = config.GetSection("targetUserId").GetChildren().Select(x => x.Value).ToList();
            var client = new GraphClient().getClient(config);

            foreach (var t in target)
            {
                int len = 0;
                List<Contact> allContacts = new List<Contact>();
                var contacts = await client.Users[t].Contacts.Request().GetAsync();
                allContacts.AddRange(contacts.CurrentPage);
                while (contacts.NextPageRequest != null)
                {
                    contacts = await contacts.NextPageRequest.GetAsync();
                    allContacts.AddRange(contacts.CurrentPage);
                }

                var ids = allContacts.Select(i => i.FileAs).ToArray();
                var count = allContacts.Count();

                foreach (Contact c in list)
                {
                    if (!ids.Contains(c.FileAs))
                    {
                        await client.Users[t].Contacts
                                .Request()
                                .AddAsync(c);
                        len++;
                    }
                }
                Logger.log("runLogger", len + " contacts posted for user " + t);
            }

        }
    }
}
