﻿using System;
using StardewModdingAPI;
using StardewValley;

namespace DecidedlyShared.Logging
{
    public class Logger
    {
        private readonly IMonitor monitor;
        private ITranslationHelper translationHelper;
        //private HashSet<> messageQueue;

        public Logger(IMonitor monitor, ITranslationHelper translationHelper)
        {
            this.monitor = monitor;
            this.translationHelper = translationHelper;
        }

        public void Log(string logMessage, LogLevel logLevel = LogLevel.Info, bool shouldAlwaysDisplayInHud = false)
        {
            this.monitor.Log(logMessage, logLevel);

            // If it's a high priority LogLevel or it's marked as should be displayed, we display it on the screen.
            if (logLevel >= LogLevel.Warn || shouldAlwaysDisplayInHud)
            {
                HUDMessage message = new(logMessage, 2);

                if (!Game1.doesHUDMessageExist(logMessage))
                    Game1.addHUDMessage(message);
            }
        }

        public void Exception(Exception e)
        {
            this.monitor.Log($"Exception: {e.Message}", LogLevel.Error);
            this.monitor.Log($"Full exception data: \n{e.Data}", LogLevel.Error);
        }
    }
}
