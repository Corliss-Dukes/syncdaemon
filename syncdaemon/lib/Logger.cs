using System;
using System.IO;

namespace syncdaemon
{
    public class Logger
    {
        ///<summary>
        /// This is the log function. It brings in a log type and a string to be logged. Based on the log type, a switch statement will route the string to be handled.
        ///</summary>
        ///<param name="logType">A string that will intiate the switch case</param>
        ///<param name="st">A string of information to be logged</param>
        public static void log(string logType, string st)
        {
            string path = @"lib\log.txt";
            switch (logType)
            {
                case "changeLog":
                    changeLog(path, st);
                    break;
                case "error":
                    errorLog(path, st);
                    break;
                case "runLogger":
                    runLogger(path, st);
                    break;
            }
        }

        // a function that routes a string to the postLog() function
        private static void runLogger(string path, string st)
        {
            postLog(path, st);
        }

        // a function that builds a string to record number of changes found
        private static void changeLog(string path, string st)
        {
            string temp = (st + " edits since last sync");
            postLog(path, temp);
        }

        // a function that logs the catch errors
        private static void errorLog(string path, string st)
        {
            string temp = ("ERROR! An error occured: " + st);
            postLog(path, temp);
        }

        ///<summary>
        /// This function appends a message to a log.txt file using StreamWriter. The current time stamp and message 
        ///</summary>
        ///<param name="path">The relative path to the log.txt file</param>
        ///<param name="st">A string of information to be logged</param>
        private static void postLog(string path, string st)
        {
            DateTime now = DateTime.Now;
            using (StreamWriter file = new StreamWriter(path, true))
            {
                file.WriteLine(now + " " + st);
            }
        }
    }
}
