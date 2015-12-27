using UnityEngine;
using System;
using System.Reflection;
using System.Text;
using System.Xml;
using System.IO;
using System.Collections;

public class GameSession {
    private int sessionID = 0;
    private string optionsFilePath = @"Data/Options.xml";
    private string levelsFilePath = @"Data/Levels/Levels.xml";
    public GameLevels CurrentLevels;
    public Options options;
    private StringBuilder BufferedLevelsXmlContents = null;

    public int SessionID
    {
        get { return this.sessionID; }
    }

    public string OptionsFilePath
    {
        get { return this.optionsFilePath; }
    }

    public GameSession()
    {
        this.sessionID = DateTime.Now.GetHashCode();
        this.CurrentLevels = new GameLevels();
        this.options = new Options();
        this.OpenLevelsFile(string.Empty);
    }

    public GameSession(GameXmlTypes _toOpen)
    {
        this.options = new Options();
        this.sessionID = DateTime.Now.GetHashCode();
        if (_toOpen == GameXmlTypes.LevelsXml)
        {
            this.OpenLevelsFile(string.Empty);
        }
        else
        {
            Logger.WriteToLog("Opening options");
        }
    }

    public void OpenOptionsFile(string _optionsFilePath)
    {
        if (_optionsFilePath.Equals(string.Empty)) _optionsFilePath = this.optionsFilePath;

        this.GetXml(_optionsFilePath, GameXmlTypes.OptionsXml);
        Logger.WriteToLog("UserAudio " + this.options.UserAudioEnabled.ToString() + " User BG " + this.options.UserBackgroundsEnabled.ToString() + " Volume level " + this.options.VolumeLevel.ToString());
    }

    public void OpenLevelsFile(string _levelsFilePath)
    {
        if (_levelsFilePath.Equals(string.Empty)) _levelsFilePath = this.levelsFilePath;

        this.GetXml(_levelsFilePath, GameXmlTypes.LevelsXml);
    }

    public bool LevelExists(string path)
    {
        for (int i = 0; i < this.CurrentLevels.TotalLevels; i++)
        {
            if (this.CurrentLevels[i].LevelPath.Equals(path)) return true;
        }
        
        return false;
    }

    public void GetXml(string xmlFilePath, GameXmlTypes fetchType = GameXmlTypes.OptionsXml)
    {
        Logger.WriteToLog("Trying to fetch options from file; " + fetchType.ToString());
        if (fetchType == GameXmlTypes.LevelsXml)
        {
            if(xmlFilePath.Equals(string.Empty)) xmlFilePath = this.levelsFilePath;
        }
        else
        { 
            if(xmlFilePath.Equals(string.Empty)) xmlFilePath = this.optionsFilePath;
        }

#if UNITY_EDITOR
        string targetPath = @"Temp";
#else
        string targetPath = @"Data/Temp";
#endif

        Logger.WriteToLog("XmlFilePath " + xmlFilePath);
        string resultFile = string.Empty;
        try
        {
            if (!Directory.Exists(targetPath)) Directory.CreateDirectory(targetPath);
            resultFile = targetPath + @"/" + DateTime.Now.GetHashCode().ToString() + @".xml";
            LevelFileCrypto.DecryptFile(xmlFilePath, resultFile, "");
        }
        catch (IOException e)
        { 
#if UNITY_EDITOR
            Debug.Log("Can't create temp folder");
            Debug.Log(e.Message);
            return;
#else
            Logger.WriteToLog("Can't create temp folder " + e.Message);
            InterfaceUpdateEventArgs ev = new InterfaceUpdateEventArgs(InterfaceUpdateReasons.ExceptionThrown, "Can't create directory", e);
            EventSystem.FireInterfaceUpdate(new object(), ev);
            return;
#endif
        }
        Logger.WriteToLog("XmlFilePath " + resultFile);
        int totalLevelsFromFile = 0;
        int levelsCount = 0;
        StringBuilder output = new StringBuilder();
        try
        {
            FileStream fs = File.Open(resultFile, FileMode.Open, FileAccess.Read, FileShare.Delete);
            using (XmlReader reader = XmlReader.Create(fs))
            {
                XmlWriterSettings ws = new XmlWriterSettings();
                ws.Indent = true;

                using (XmlWriter writer = XmlWriter.Create(output, ws))
                {
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:
                                {
                                    if (fetchType == GameXmlTypes.OptionsXml)
                                    {
                                        Logger.WriteToLog("Fetching options");
                                        writer.WriteStartElement(reader.Name);
                                        if (reader.Name.Equals("option"))
                                        {
                                            if (reader.HasAttributes)
                                            {
                                                Logger.WriteToLog("Reading options attributes");
                                                string name = string.Empty;
                                                string value = string.Empty;
                                                while (reader.MoveToNextAttribute())
                                                {
                                                    Logger.WriteToLog("Attrib " + reader.Name + " " + reader.Value);
                                                    if (reader.Name.Equals("name")) name = reader.Value;
                                                    if (reader.Name.Equals("value")) value = reader.Value;
                                                    writer.WriteAttributeString(reader.Name, reader.Value);                                                    
                                                }
                                                this.options[name] = value;
                                                reader.MoveToElement();
                                                writer.WriteEndElement();
                                            }
                                            else
                                                throw new Exception("Element has no attributes");
                                        }
                                    }
                                    else if (fetchType == GameXmlTypes.LevelsXml)
                                    {
                                        writer.WriteStartElement(reader.Name);
                                        if (reader.Name.Equals("levelFiles"))
                                        {
                                            if (reader.HasAttributes)
                                            {
                                                if (!int.TryParse(reader.GetAttribute(0), out totalLevelsFromFile))
                                                    throw new Exception("No integer value in attribute");
                                                writer.WriteAttributeString("totalLevels", totalLevelsFromFile.ToString());
                                            }
                                            else
                                                throw new Exception("Root element has no attributes");
                                        }
                                        else
                                        {
                                            GameLevel gl = new GameLevel();
                                            if (reader.HasAttributes)
                                            {
                                                while (reader.MoveToNextAttribute())
                                                {
                                                    gl[reader.Name] = reader.Value.ToString();
                                                    writer.WriteAttributeString(reader.Name, reader.Value);
                                                }
                                                reader.MoveToElement();
                                                writer.WriteEndElement();
                                            }
                                            else
                                                throw new Exception("Element has no attributes");
                                            this.CurrentLevels[levelsCount] = gl;
                                            levelsCount++;
                                        }
                                    }
                                }
                                break;
                            case XmlNodeType.XmlDeclaration: writer.WriteProcessingInstruction(reader.Name, reader.Value); break;
                            case XmlNodeType.EndElement:
                                {
                                    if (!reader.Name.Equals("levelFile") && !reader.Name.Equals("option"))
                                        writer.WriteEndElement();
                                }
                                break;
                        }
                    }
                }
            }
            fs.Close();
            fs.Dispose();
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.Log("Problem reading xml!");
            Debug.Log(e.Message);
            return;
#else
            Logger.WriteToLog("Problem reading xml! " + e.Message);
            InterfaceUpdateEventArgs ev = new InterfaceUpdateEventArgs(InterfaceUpdateReasons.ExceptionThrown, "Can't open levels xml", e);
            EventSystem.FireInterfaceUpdate("fuck", ev);
            return;
#endif
        }
        finally
        {
            this.BufferedLevelsXmlContents = output;
            
            if (File.Exists(resultFile))
            {
                try
                {
                    File.Delete(resultFile);
                }
                catch (IOException e)
                {
                    Debug.Log("Nested try, what a fuck man?!");
                    Debug.Log(e.Message);
                }
            }
        }
    }

    private void SetOption(string name, string value)
    { 
        
    }

    #region write_xml

    public void WriteXml(string xmlFilePath, GameXmlTypes fetchType = GameXmlTypes.OptionsXml)
    {
        Logger.WriteToLog("Writing to xml");
        if (fetchType == GameXmlTypes.LevelsXml)
        {
            if (xmlFilePath.Equals(string.Empty)) xmlFilePath = this.levelsFilePath;
        }
        else
        {
            if (xmlFilePath.Equals(string.Empty)) xmlFilePath = this.optionsFilePath;
        }

#if UNITY_EDITOR
        string targetPath = xmlFilePath;
        string targetFolder = @"Temp";
#else
        string targetFolder = @"Data/Temp";
        string targetPath = xmlFilePath;
#endif

        string tempFile = string.Empty;
        Logger.WriteToLog("Target option file " + xmlFilePath);
        try
        {
            if (!Directory.Exists(targetFolder)) Directory.CreateDirectory(targetFolder);
            tempFile = targetFolder + @"/" + DateTime.Now.GetHashCode().ToString() + @".xml";
        }
        catch (IOException e)
        {
#if UNITY_EDITOR
            Debug.Log("Can't create temp folder");
            Debug.Log(e.Message);
            return;
#else
            Logger.WriteToLog("Can't create temp folder " + e.Message);
            InterfaceUpdateEventArgs ev = new InterfaceUpdateEventArgs(InterfaceUpdateReasons.ExceptionThrown, "Can't create directory", e);
            EventSystem.FireInterfaceUpdate(new object(), ev);
            return;
#endif
        }

        StringBuilder ResultString = new StringBuilder();
        string fileNewTemp = string.Empty;
        try
        {
            if (fetchType == GameXmlTypes.LevelsXml)
            {
                ResultString.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                ResultString.AppendLine("<levelFiles totalLevels=\"" + this.CurrentLevels.TotalLevels.ToString() + "\">");
                for (int i = 0; i < this.CurrentLevels.TotalLevels; i++)
                {
                    ResultString.AppendLine("\t<levelFile encrypted=\"" + (this.CurrentLevels[i].LevelEncrypted ? (1).ToString() : (0).ToString()) + "\" filePath=\"" + this.CurrentLevels[i].LevelPath + "\" levelNumber=\"" + this.CurrentLevels[i].LevelNumber + "\" userLevel=\"" + (this.CurrentLevels[i].UserLevel ? (1).ToString() : (0).ToString()) + "\" />");
                }
                ResultString.AppendLine("</levelFiles>");

                if (ResultString.ToString().Equals(string.Empty)) throw new Exception("No result string.");

                byte[] resultBytes = ASCIIEncoding.ASCII.GetBytes(ResultString.ToString());
                Debug.Log("Temp file:" + tempFile);
                FileStream fs = new FileStream(tempFile, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                
                if (fs == null) throw new Exception("Error creating file");
                fs.Close();
                File.WriteAllBytes(tempFile, resultBytes);

                fileNewTemp = targetFolder + @"/" + DateTime.Now.GetHashCode().ToString() + @".xml";
            }
            else
            {
                Type myType = typeof(Options);
                FieldInfo[] fi = myType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
                ResultString.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                ResultString.AppendLine("<options>");
                foreach (FieldInfo f in fi)
                {
                    Logger.WriteToLog("<option name=\"" + f.Name + "\" value=\"" + f.GetValue(this.options).ToString() + "\" />");
                    ResultString.AppendLine("<option name=\"" + f.Name +"\" value=\"" + f.GetValue(this.options).ToString() + "\" />");   
                }
                ResultString.AppendLine("</options>");

                if (ResultString.ToString().Equals(string.Empty)) throw new Exception("No result string.");

                byte[] resultBytes = ASCIIEncoding.ASCII.GetBytes(ResultString.ToString());
                FileStream fs = new FileStream(tempFile, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);

                if (fs == null) throw new Exception("Error creating file");
                fs.Close();
                
                File.WriteAllBytes(tempFile, resultBytes);
                Logger.WriteToLog("Temp file phase passed?");
                fileNewTemp = targetFolder + @"/" + DateTime.Now.GetHashCode().ToString() + @".xml";
            }

            LevelFileCrypto.EncryptFile(tempFile, fileNewTemp, "");
            Logger.WriteToLog("Target path " + targetPath);
            byte[] encryptedBytes = File.ReadAllBytes(fileNewTemp);
            File.Delete(targetPath);
            File.WriteAllBytes(targetPath, encryptedBytes);
            Logger.WriteToLog("All bytes written");
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.Log("Can't write bytes to file");
            Debug.Log(e.Message);
            return;
#else
            Logger.WriteToLog("Can't write bytes to file " + e.Message);
            InterfaceUpdateEventArgs ev = new InterfaceUpdateEventArgs(InterfaceUpdateReasons.ExceptionThrown, "Can't create directory", e);
            EventSystem.FireInterfaceUpdate(new object(), ev);
            return;
#endif
        }
        finally
        {
            try
            {
                File.Delete(tempFile);
                File.Delete(fileNewTemp);
            }
            catch (Exception ex)
            {
#if UNITY_EDITOR
                Debug.Log("Second nested try? Seriously?");
                Debug.Log(ex.Message);
#else
                Logger.WriteToLog("Second nested try? Seriously? " + ex.Message);
                InterfaceUpdateEventArgs ev = new InterfaceUpdateEventArgs(InterfaceUpdateReasons.ExceptionThrown, "Can't create directory", ex);
                EventSystem.FireInterfaceUpdate(new object(), ev);
#endif
            }
        }
    }
    #endregion
}
