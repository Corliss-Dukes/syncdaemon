** To run program `dotnet run --Daemon:DaemonName="Sync Daemon"`
** To quit program ` Ctrl + c `
Author - Gregory Dukes www.gregorydukes.com

As of this current version, Teams will only pull contacts from the default Contacts folder in Outlook.  There is not a way that I could find to create a shared contact list, so the app pushes changes to each relevant User in the Active Directory.  The app will look for a contact based on a unique identifier, so it should be okay if users mix personal contacts in with the patients list.  Patient Contact objects will store the corliss Account ID as the MS FileAs parameter.  This will ensure a common unique identifier between the two contact lists. 



_User IDs_
Arlene - 50a12f90-6a50-4793-8cd9-fb2496f04ab4
Emalee - 2594346b-c338-4358-acad-b754e614bb7b
Ken  -  03c23af9-46ea-4074-86b5-2b35bc7c1bee
Midge - 78e1171f-e15c-45f7-bb64-6a29c30853a5
Nikki - 5a8817e4-b012-4180-ba75-18fe3e8b4659
Greg - c46ee1b2-b28b-4783-bfce-a79e17c26450



Code sources...
https://medium.com/@NitinManju/a-simple-scheduled-task-using-c-and-net-c9d3230769ea
https://www.wintellect.com/creating-a-daemon-with-net-core-part-1/
https://docs.microsoft.com/en-us/graph/?view=graph-rest-1.0