using UnityEngine;
using System;
using System.Text;
using System.Xml;
using System.IO;
using System.Collections;

public class GameSession {
    private int sessionID = 0;
    private string optionsFilePath = @"Data/Options.xml";
    private string levelsFilePath = @"Data/Levels/Levels.xml";
    public GameLevels CurrentLevels;
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
        this.OpenLevelsFile(string.Empty);
    }

    public void OpenOptionsFile(string _optionsFilePath)
    {
        if (_optionsFilePath.Equals(string.Empty)) _optionsFilePath = this.optionsFilePath;

        this.GetXml(_optionsFilePath, GameXmlTypes.OptionsXml);

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
            InterfaceUpdateEventArgs ev = new InterfaceUpdateEventArgs(InterfaceUpdateReasons.ExceptionThrown, "Can't create directory", e);
            EventSystem.FireInterfaceUpdate(new object(), ev);
            return;
#endif
        }

        this.CurrentLevels = new GameLevels();
        int totalLevelsFromFile = 0;
        int levelsCount = 0;
        StringBuilder output = new StringBuilder();
        try
        {
            using (XmlReader reader = XmlReader.Create(File.Open(resultFile, FileMode.Open, FileAccess.Read, FileShare.Delete)))
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
                                        
                                    }
                                    else if (fetchType == GameXmlTypes.LevelsXml)
                                    {
                                        writer.WriteStartElement(reader.Name);
                                        Debug.Log(reader.Name);
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
                                                    Debug.Log(reader.Name + " " + reader.Value.ToString());
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
                                    if (!reader.Name.Equals("levelFile"))
                                        writer.WriteEndElement();
                                }
                                break;
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.Log("Problem reading xml!");
            Debug.Log(e.Message);
            return;
#else
            InterfaceUpdateEventArgs ev = new InterfaceUpdateEventArgs(InterfaceUpdateReasons.ExceptionThrown, "Can't open levels xml", e);
            EventSystem.FireInterfaceUpdate("fuck", ev);
            return;
#endif
        }
        finally
        {
            this.BufferedLevelsXmlContents = output;
            Debug.Log(output);
            Debug.Log(totalLevelsFromFile.ToString() + " " + this.CurrentLevels.TotalLevels.ToString());
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

    public void WriteXml(string xmlFilePath, GameXmlTypes fetchType = GameXmlTypes.OptionsXml)
    {
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
                
            }

            LevelFileCrypto.EncryptFile(tempFile, fileNewTemp, "");

            byte[] encryptedBytes = File.ReadAllBytes(fileNewTemp);
            File.WriteAllBytes(targetPath, encryptedBytes);
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.Log("Can't write bytes to file");
            Debug.Log(e.Message);
            return;
#else
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
            catch (Exception ev)
            {
#if UNITY_EDITOR
                Debug.Log("Second nested try? Seriously?");
                Debug.Log(ev.Message);
#else
                InterfaceUpdateEventArgs ev = new InterfaceUpdateEventArgs(InterfaceUpdateReasons.ExceptionThrown, "Can't create directory", e);
                EventSystem.FireInterfaceUpdate(new object(), ev);
                return;
#endif
            }
        }
    }
}
