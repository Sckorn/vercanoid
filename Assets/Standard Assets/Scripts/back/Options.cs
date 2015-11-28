using UnityEngine;
using System;
using System.Reflection;
using System.IO;
using System.Collections;
using System.Xml;

public class Options {

    private string sOptionsFilePath = @"Data/Options.xml";
    private bool bEnableUserBackgrounds = true;
    private bool bEnableUserAudio = true;
    private float fAudioVolumeLevel = 1.0f;  //percentage 100 == 100%

    public Options()
    {
        try
        {
            this.FetchOptionsFromFile();
        }
        catch (Exception e)
        { 
#if UNITY_EDITOR
            Debug.Log("Can't fetch options from file");
            Debug.Log(e.Message);
#else
            Logger.WriteToLog("Can't fetch options from file " + e.Message);
#endif
        }
    }

    public Options(bool _userBG, bool _userAudio, float _volume)
    {
        this.bEnableUserAudio = _userAudio;
        this.bEnableUserBackgrounds = _userBG;
        this.fAudioVolumeLevel = _volume;
    }

    public bool UserBackgroundsEnabled
    {
        get { return this.bEnableUserBackgrounds; }
        set { this.bEnableUserBackgrounds = value; }
    }

    public bool UserAudioEnabled
    {
        get { return this.bEnableUserAudio; }
        set { this.bEnableUserAudio = value; }
    }

    public float VolumeLevel
    {
        get { return this.fAudioVolumeLevel; }
        set { this.fAudioVolumeLevel = value; }
    }

    public string this[string param]
    {
        get
        {
            if (param.Equals("bEnableUserAudio"))
            {
                return this.bEnableUserAudio.ToString();
            }
            else if (param.Equals("bEnableUserBackgrounds"))
            {
                return this.bEnableUserBackgrounds.ToString();
            }
            else if (param.Equals("fAudioVolumeLevel"))
            {
                return this.fAudioVolumeLevel.ToString();
            }

            return string.Empty;
        }

        set
        {
            if (param.Equals("bEnableUserAudio"))
            {
                int extra;
                bool tmp;
                if (!bool.TryParse(value, out tmp))
                {
                    if (!int.TryParse(value, out extra))
                    {
                        throw new Exception("Bad incoming value");
                    }

                    if(extra > 0) tmp = true;
                    else tmp = false;
                }

                this.bEnableUserAudio = tmp;
            }
            else if (param.Equals("bEnableUserBackgrounds"))
            {
                int extra;
                bool tmp;
                if (!bool.TryParse(value, out tmp))
                {
                    if (!int.TryParse(value, out extra))
                    {
                        throw new Exception("Bad incoming value");
                    }

                    if (extra > 0) tmp = true;
                    else tmp = false;
                }

                this.bEnableUserBackgrounds = tmp;
            }
            else if (param.Equals("fAudioVolumeLevel"))
            {
                float tmp;

                if (!float.TryParse(value, out tmp))
                {
                    throw new Exception("Bad incoming value");
                }

                this.fAudioVolumeLevel = tmp;
            }
        }
    }

    private void FetchOptionsFromFile()
    {
#if UNITY_EDITOR
        string targetFolder = "Temp";
#else
        string targetFolder = "Data/Temp";
#endif
        string resultPath = targetFolder + "/" + DateTime.Now.GetHashCode().ToString() + ".xml";

        if (!File.Exists(this.sOptionsFilePath))
        {
            try
            {
                this.WriteOptions();
            }
            catch (Exception e)
            { 
#if UNITY_EDITOR
                Debug.Log("Can't create generic options file");
                Debug.Log(e.Message);
                return;                
#else
                Logger.WriteToLog("Can't create generic options file " + e.Message);
                return;
#endif
            }
        }

        try
        {
            LevelFileCrypto.DecryptFile(this.sOptionsFilePath, resultPath, "");
            FileStream fs = File.Open(resultPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

            using (XmlReader reader = XmlReader.Create(fs))
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            {
                                if (reader.Name.Equals("option"))
                                {
                                    if (reader.HasAttributes)
                                    {
                                        string name = string.Empty;
                                        string value = string.Empty;
                                        while (reader.MoveToNextAttribute())
                                        {
                                            if (reader.Name.Equals("name")) name = reader.Value;
                                            if (reader.Name.Equals("value")) value = reader.Value;
                                        }
                                        this[name] = value;
                                        reader.MoveToElement();
                                    }
                                    else
                                        throw new Exception("Element has no attributes");
                                }
                            }
                            break;
                    }
                }
            }

            fs.Close();
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.Log("Can't read options xml file");
            Debug.Log(e.Message);
            return;
#else
            Logger.WriteToLog("Can't read options xml file " + e.Message, this);
#endif
        }
        finally
        {
            try
            {
                File.Delete(resultPath);
            }
            catch (Exception ex)
            { 
#if UNITY_EDITOR
                Debug.Log("Can't delete temp file");
                Debug.Log(ex.Message);
#else
                Logger.WriteToLog("Can't delete temp file " + ex.Message);
#endif
            }
        }
    }

    public void WriteOptions()
    {
#if UNITY_EDITOR
        string targetFolder = "Temp";
#else
        string targetFolder = "Data/Temp";
#endif

        string resultPath = targetFolder + "/" + DateTime.Now.GetHashCode().ToString() + ".xml";

        try
        {
            FileStream fs = File.Open(resultPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            Type optionsType = typeof(Options);
            FieldInfo[] fi = optionsType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                sw.WriteLine("<options>");
                foreach (FieldInfo f in fi)
                {
                    if (!f.Name.Equals("sOptionsFilePath"))
                        sw.WriteLine("\t<option name=\"" + f.Name + "\" value=\"" + f.GetValue(this).ToString() + "\" />");
                }
                sw.WriteLine("</options>");
            }
            fs.Close();
            LevelFileCrypto.EncryptFile(resultPath, this.sOptionsFilePath, "");
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.Log("Can't create options file.");
            Debug.Log(e.Message);
            return;
#else
            Logger.WriteToLog("Can't create options file. " + e.Message, this);
#endif
        }
        finally
        {
            try
            {
                File.Delete(resultPath);
            }
            catch (Exception ex)
            { 
#if UNITY_EDITOR
                Debug.Log("Can't delete temp file");
                Debug.Log(ex.Message);
#else
                Logger.WriteToLog("Can't delete temp file " + ex.Message, this);
#endif

            }
        }
    }
}