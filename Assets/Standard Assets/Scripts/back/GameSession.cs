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
    private GameLevels CurrentLevels;

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
        if (optionsFilePath.Equals(string.Empty)) _optionsFilePath = this.optionsFilePath;

        GameSession.GetXml(_optionsFilePath, GameXmlTypes.OptionsXml);

    }

    public void OpenLevelsFile(string _levelsFilePath)
    {
        if (optionsFilePath.Equals(string.Empty)) _levelsFilePath = this.levelsFilePath;

        GameSession.GetXml(_levelsFilePath, GameXmlTypes.LevelsXml);
    }

    static public void GetXml(string xmlFilePath, GameXmlTypes fetchType = GameXmlTypes.OptionsXml)
    {
        if (xmlFilePath.Equals(string.Empty)) throw new Exception("Argument null exception");

        StringBuilder output = new StringBuilder();
        using (XmlReader reader = XmlReader.Create(File.Open(xmlFilePath, FileMode.Open, FileAccess.Read, FileShare.Read)))
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
                                    
                                }
                            }
                            break;
                        case XmlNodeType.Attribute:
                            {
                                if (fetchType == GameXmlTypes.OptionsXml)
                                {

                                }
                                else if (fetchType == GameXmlTypes.LevelsXml)
                                {

                                }
                            }
                            break;
                        case XmlNodeType.XmlDeclaration:
                        case XmlNodeType.ProcessingInstruction:
                            {
                                if (fetchType == GameXmlTypes.OptionsXml)
                                {

                                }
                                else if (fetchType == GameXmlTypes.LevelsXml)
                                {

                                }
                            }
                            break;
                        case XmlNodeType.EndElement:
                            {
                                if (fetchType == GameXmlTypes.OptionsXml)
                                {

                                }
                                else if (fetchType == GameXmlTypes.LevelsXml)
                                {

                                }
                            }
                            break;
                    }
                }
            }
        }
    }
}
