using UnityEngine;
using System;
using System.Collections;
using System.IO;

public static class Logger {

    public static FileStream logFileHandle = null;
    public static string logFilePath = string.Empty;

    public static void CreateLogFile()
    {
        string basePath = string.Empty;

#if UNITY_EDITOR
        basePath = @"Temp";
#else
        basePath = @"Data/Temp";
#endif

        if (!Directory.Exists(basePath)) Directory.CreateDirectory(basePath);

        string resultFilePath = basePath + @"/Log" + DateTime.Now.GetHashCode().ToString() + @".log";
        FileStream fs = null;
        try
        {
            fs = File.Create(resultFilePath);
        }
        catch (Exception e)
        {
            Application.Quit(); /* later change to handling and quit */
        }        

        Logger.logFileHandle = fs;
    }

    public static void RemoveAllExistingLogs()
    {
#if UNITY_EDITOR
        string[] files = Directory.GetFiles(@"Temp", "*.log");
#else
        string[] files = Directory.GetFiles(@"Data/Temp", "*.log");
#endif

        foreach (string str in files)
        {
            try
            {
                File.Delete(str);
            }
            catch (Exception e)
            {
                Debug.Log("Can't delete log file");
                Debug.Log(e.Message);
                return;
            }
        }
    }

    public static void WriteToLog(string message)
    {
        if (Logger.logFileHandle != null)
        {
            using (StreamWriter sr = new StreamWriter(Logger.logFileHandle))
            {
                string prefix = string.Empty;
                prefix += "[ ";
                prefix += DateTime.Now.ToLongDateString() + @" ||| " + Time.time.ToString();
                prefix += " ]:";
                string postfix = ";";
                sr.WriteLine(prefix + message + postfix);
            }
        }
    }

    public static void CloseLogFileHandle()
    {
        Logger.logFileHandle.Close();
    }

}
