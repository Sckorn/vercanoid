using UnityEngine;
using System;
using System.Diagnostics;
using System.Text;
using System.Collections;
using System.IO;

public static class Logger {

    public static FileStream logFileHandle = null;
    public static string logFilePath = string.Empty;
    public static bool logExists = false;

    public static void CreateLogFile()
    {
        Logger.RemoveAllExistingLogs();
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
            fs = new FileStream(resultFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Delete);
        }
        catch (Exception e)
        {
            Application.Quit(); /* later change to handling and quit */
        }

        Logger.logFileHandle = fs;
        Logger.logExists = true;
        Logger.WriteToLog("Log file created");
    }

    public static void RemoveAllExistingLogs()
    {
        //Logger.logFileHandle.Close();
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
                UnityEngine.Debug.Log("Can't delete log file");
                UnityEngine.Debug.Log(e.Message);
                return;
            }
        }
    }

    public static void WriteToLog(string message)
    {
        if (Logger.logFileHandle != null)
        {
            string prefix = string.Empty;
            prefix += "[ ";
            prefix += DateTime.Now.Date.ToShortDateString() + " " + DateTime.Now.TimeOfDay.ToString() + @" ||| " + Time.time.ToString();
            prefix += " ]:";
            string postfix = ";\n";
            string result = prefix + message + postfix;
            byte[] bytesMessage = ASCIIEncoding.ASCII.GetBytes(result);
            Logger.logFileHandle.Write(bytesMessage, 0, bytesMessage.Length);
        }
    }

    public static int GetExceptionsLineNumber(Exception e)
    {
        int lineNumber = 0;
        const string lineSearch = ":line ";
        int index = e.StackTrace.LastIndexOf(lineSearch);
        if (index != -1)
        {
            string LineNumberText = e.StackTrace.Substring(index + lineSearch.Length);
            if (int.TryParse(LineNumberText, out lineNumber))
            {
                return lineNumber;
            }
        }

        return -1;
    }

    public static void WriteToLog(string message, object instigator)
    {
        if (Logger.logFileHandle != null)
        {
            string prefix = string.Empty;
            prefix += "[ ";
            prefix += DateTime.Now.Date.ToShortDateString() + " " + DateTime.Now.TimeOfDay.ToString() + @" ||| " + Time.time.ToString();
            prefix += " ]:";
            string postfix = "\t From: " + instigator.ToString() + ";\n";
            string result = prefix + message + postfix;
            byte[] bytesMessage = ASCIIEncoding.ASCII.GetBytes(result);
            Logger.logFileHandle.Write(bytesMessage, 0, bytesMessage.Length);
        }
    }

    public static void CloseLogFileHandle()
    {
        Logger.logFileHandle.Close();
    }

}
