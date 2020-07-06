using System;
using System.Threading.Tasks;
using Microsoft.Graph;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace syncdaemon
{
    public class NightlySync
    {
        ///<summary>
        /// This is the main method of the app. First it queries the db to get a list of updated patient info.
        /// If the list count is not 0, it runs the 2 helper methods to process the contact and post it to MS Contacts.
        ///</summary>
        ///<param name="config">The configuration object</param>
        public void RunSync(IConfigurationRoot config)
        {
            var plist = fetchChangelist(config);
            if (plist.Count() > 0)
            {
                var clist = processChangeList(plist);
                postChangelist(clist, config);
            }
        }

        ///<summary>
        ///This method makes a sql call and grabs any Patients that have been edited in the last 24hrs
        ///</summary>
        ///<param name="config">The configuration object</param>
        ///<returns>List{Patient}</returns>
        static List<Patient> fetchChangelist(IConfigurationRoot config)
        {
            using (var db = new AccTestContext(config))
            {
                var list = db.Patient.Select(p => p)
                           .Where(p => p.TodaysDate > DateTime.Now.AddHours(-24)).ToList();
                Logger.log("changeLog", list.Count().ToString());
                return list;
            }
        }

        ///<summary>
        ///This method brings in a List of Patient objects from SQL and processes them into MS Contact objects
        ///</summary>
        ///<param name="plist">The list of Patients to be processed</param>
        ///<returns>List{Contact}</returns>
        static List<Contact> processChangeList(List<Patient> plist)
        {
            var list = new List<Contact>();
            foreach (var p in plist)
            {
                list.Add(new PatientContact().NewContact(p));
            }
            return list;
        }

        ///<summary>
        ///This method will loop over a list of Corliss Users and for each User get all their contacts, set updated contact ids, delete any old contacts, and add the new or edited contact.
        ///</summary>
        ///<param name="list">The newly processed list of MS Contact objects</param>
        ///<param name="config">The configuration object</param>
        static async void postChangelist(List<Contact> list, IConfigurationRoot config)
        {
            var target = config.GetSection("targetUserId").GetChildren().Select(x => x.Value).ToList();
            var client = new GraphClient().getClient(config);
            foreach (var t in target) //for each Corliss User
            {
                //this section sets up a list of ms Ids and fileAs from the User MS Contacts to compare against
                List<Contact> allOldContacts = new List<Contact>();
                var oldContacts = await client.Users[t].Contacts
                    .Request().Select("id,fileAs").GetAsync();

                allOldContacts.AddRange(oldContacts.CurrentPage);
                //Graph returns pages of results, this is how we handle the pages
                while (oldContacts.NextPageRequest != null)
                {
                    oldContacts = await oldContacts.NextPageRequest.GetAsync();
                    allOldContacts.AddRange(oldContacts.CurrentPage);
                }

                //Add existing MS Contact ID to any edited contacts
                //TODO: this should return a new List, not change the existing List
                if (allOldContacts.Count > 0)
                {
                    foreach (var c in list)
                    {
                        var n = allOldContacts.Where(o => o.FileAs == c.FileAs).FirstOrDefault();
                        if (n != null) c.Id = n.Id;
                    }
                }

                //this section will loop through the list check for null ID, delete ms Contact @ ID if !null, then add Contact
                int count = 0;
                foreach (var x in list)
                {
                    if (x.Id != null)
                    {
                        //delete from MS Contacts by ID and Save the updated Contact in MS Contacts
                        bool y = await deleteOldContact(t, client, x.Id)
                                ? await addUpdatedContact(t, client, x)
                                : false;
                        if (y) count++;
                    }
                    else
                    {
                        //add new contacts to MS Contacts
                        bool y = await addUpdatedContact(t, client, x)
                                ? true
                                : false;
                        if (y) count++;
                    }
                }
                string st = (count.ToString() + " contacts synced for User " + t);
                Logger.log("runLogger", st);
            }
        }

        // ***************************** HELPER FUNCTIONS **************************

        ///<summary>
        /// This is a helper function for postChangelist(). This function uses the client to access the User's contacts then deletes the contact the matches the contact ID 
        ///</summary>
        ///<param name="target">The User ID</param>
        ///<param name="client">The GraphServiceClient that is used for Graph api access</param>
        ///<param name="id">The Contact Id used to identify the correct Contact to delete</param>
        ///<returns>Task{boolean}</returns>
        static async Task<bool> deleteOldContact(string target, GraphServiceClient client, string id)
        {
            try
            {
                await client.Users[target].Contacts[id]
                .Request().DeleteAsync();
                return true;
            }
            catch (Exception e)
            {
                Logger.log("errorLog", e.Message);
                return false;
            }
        }
        ///<summary>
        /// This is a helper function for postChangelist(). This function brings in a Contact object and uses the client to access the User's contacts, then posts the new contact to the Users contacts
        ///</summary>
        ///<param name="target">The User ID</param>
        ///<param name="client">The GraphServiceClient that is used for Graph access</param>
        ///<param name="c">The Contact Object to add to the User's Contact list</param>
        ///<returns>Task{boolean}</returns>
        static async Task<bool> addUpdatedContact(string target, GraphServiceClient client, Contact c)
        {
            try
            {
                await client.Users[target].Contacts
                .Request().AddAsync(c);
                return true;
            }
            catch (Exception e)
            {
                Logger.log("errorLog", e.Message);
                return false;
            }
        }
    }
}
