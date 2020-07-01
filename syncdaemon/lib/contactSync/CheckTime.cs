﻿using System;
using Microsoft.Extensions.Configuration;

namespace syncdaemon
{
    class CheckTime
    {
        ///<summary>
        /// This will run a dateTime check. If it is 11:00 PM (2300 hrs) it will run the NightlySync() function.
        ///</summary>
        ///<param name="config">The IConfigurationRoot object that passes to the NightlySync() function</param>
        public void RunCheck(IConfigurationRoot config)
        {
            DateTime dt = DateTime.Now;
            Console.WriteLine(dt + " execute RunCheck");
            if (dt.TimeOfDay.Hours == 23)
            {
                Logger.log("runLogger", "Initialize NightlySync()");
                new NightlySync().RunSync(config);
            }
        }
    }
}
