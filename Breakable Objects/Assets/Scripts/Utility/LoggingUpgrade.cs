using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Utility
{
    class LoggingUpgrade
    {
        private const String FILE_NAME = "log.txt";

        private static void  HandleLog(string logString, string stackTrace, LogType type)
        {
            using (StreamWriter w = File.AppendText(FILE_NAME))
            {
                w.WriteLine(logString);
            }
        }

        public static void RegisterLogFile()
        {
            Debug.unityLogger.logEnabled = false;
            Application.logMessageReceived += HandleLog;
            File.WriteAllText(FILE_NAME, String.Empty);
        }
    }
}
