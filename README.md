# Contacts Sync Daemon
---------------------------------

## a Service Daemon application

The service daemon consists of a single C# console project that runs in the background on a desktop or server. 
The interval that the program waits between action executions can be set to any interval in seconds, currently it 
performs a `TimeCheck` action every hour, looking for a match for 2300 hours.  At 2300, the ```TimeCheck``` function
calls the `NightlySync` function which is the main function this daemon was built to perform.

The `NightlySync` function will first run a query against the designated SQL Database to look for contacts that have
been edited since that last sync was performed. If no changes have been made, the function ends and the app continues 
to run `TimeCheck` every hour.
If a change has been found, the contact is returned, gets processed into a [Microsoft Contact](https://docs.microsoft.com/en-us/graph/api/resources/contact?view=graph-rest-1.0) 
JSON object, and posted to the Default Contact folder in every Users' Outlook directory using the Microsoft Graph API.

As of this current version, Teams will only pull contacts from the default Contacts folder in Outlook.  There is not
 a way that I could find to create a shared contact list, so the app pushes changes to each relevant User in the 
Active Directory.  The app will look for a contact based on a unique identifier, so it should be okay if users mix 
personal contacts in with the patients list.  Microsoft Contact objects will store the SQL ID as the FileAs 
parameter.  This will ensure a common unique identifier between the two contact lists. 


---------------------------------

## Tools Used
Microsoft Visual Studio Community 2019 (Version 16.5.0)

- C#
- .Net Core
- Entity Framework
- Azure AD
- [Microsoft Graph API](https://docs.microsoft.com/en-us/graph/?view=graph-rest-1.0)
- [Graph API tutorials](https://developer.microsoft.com/en-us/graph/get-started/dotnet-core)
- [Microsoft Graph Explorer](https://developer.microsoft.com/en-us/graph/graph-explorer)


#### Code sources...
- [Create a daemon](https://www.wintellect.com/creating-a-daemon-with-net-core-part-1/)
- [Schedule a task](https://medium.com/@NitinManju/a-simple-scheduled-task-using-c-and-net-c9d3230769ea)

---------------------------------

## Recent Updates

#### V 1.2
*Added a dedicated email validator, separate from `NightlySync()` in the `TimeCheck()`* - 9 Jul 2020

---------------------------

## Getting Started

Clone this repository to your local machine.
```
$ git clone https://github.com/Corliss-Dukes/syncdaemon
```
Once downloaded, you can either use the dotnet CLI utilities or Visual Studio 2019 (or greater) to build the web 
application. The solution file is located in the syncdaemon subdirectory at the root of the repository.
```
cd syncdaemon/syncdaemon
dotnet build
```
The dotnet tools will automatically restore any dependencies. Before running the application, you will need to supply 
an `appsettings.json` file in the same directory as the solution which includes your "tenantId", "applicationID",
"applicationSecret", database connection string, and an array of Azure AD User Ids to target for contacts sync.  These
can all be found in the Azure AD Portal and when giving this app permission to access your Azure AD.  Steps can be found
in the tutorials mentioned above.

```
{
  "tenantId": "xxxx...",
  "applicationId": "xxxx...",
  "applicationSecret": "xxxx...",
  "AccTestDB": "Server=xxxx;Database=xxxx;User Id=xxxx;Password=xxxx;",
  "targetUserId": [ "xxxx...", "xxxx..." ]
}
```
Once the `appsettings.json` has been created and the app registered with permissions on Azure AD, the application can be run. Options for running and debugging the application using IIS Express or Kestrel are provided within Visual Studio. From the command line, the following will start an instance of the Kestrel server to host the application:
```
cd syncdaemon/syncdaemon
dotnet run --Daemon:DaemonName="Sync Daemon"
```

To quit the application using the command line:
```
 Ctrl + c
```

---------------------------------

## Usage

### Starting app with command line
![app running in command line](/syncdaemon/lib/assets/dotnetRun.PNG)

### Example log.txt file
![example log.txt file](/syncdaemon/lib/assets/log.PNG)


---------------------------
## Data Flow 
* At app start, the app begins executing `TimeCheck` every hour
* `TimeCheck` looks for 2300 hours.
* At 2300 hours, `TimeCheck` executes `NightlySync`.
* `NightlySync` queries SQL Database, returns all contacts that have been edited since last sync.
* Converts SQL contact object to a Microsoft Graph Contact object.
* Using MS Graph API, for each User, queries Microsoft Outlook Default Contact folder
    * Delete contact where IDs match
    * Add new contact
* Logs changes  
---------------------------


## Change Log
* 1.1: Added an `InitialSync()` function to fill all Users' contact lists* - 30 Jun 2020
* 1.2: Added a dedicated email validator, separate from `NightlySync()` in the `TimeCheck()` - 9 Jul 2020

------------------------------

## Author
Gregory Dukes <br>
www.gregorydukes.com