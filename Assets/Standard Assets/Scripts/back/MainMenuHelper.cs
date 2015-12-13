#define DEBUG_BUILD
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Reflection;
using System.IO;
using System.Collections;
using SimpleFileCrypter;

public class MainMenuHelper : MonoBehaviour {
    private string optionsFilePath = @"Data/Options.ini";
    private Options options;
    private GameSession optionsOnlySession;

    /*[System.Runtime.InteropServices.DllImport(@"SimpleFileCrypter.dll")]
    static public extern void EncryptFileFromString(string sInputString, string sOutputFilePath, string sKey);*/

    void Awake()
    {
        if(Globals.options == null)
            Globals.InitializeOptions();

#if DEBUG_BUILD
        if (!Logger.logExists)
        {
            Logger.CreateLogFile();
            Logger.WriteToLog("Game started");
            Logger.WriteToLog("Main Menu Scene loaded");
        }
#endif
    }

	// Use this for initialization
	void Start () {
        //this.optionsOnlySession = new GameSession(GameXmlTypes.OptionsXml);
        this.ResizeHeader();
        //this.OpenOptionsFile();
        Debug.Log("Volume level " + Globals.options.VolumeLevel.ToString());
        StartCoroutine("OptionsInterfaceUpdateRoutine");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ResizeHeader()
    {
        RectTransform rt = GameObject.Find("HeaderPanel").GetComponentInChildren<RectTransform>();
        float quarterOfHeight = Screen.height / 4;
        rt.sizeDelta = new Vector2(Screen.width, quarterOfHeight);
    }

    public void StartSinglePlayer()
    {
        try
        {
            EventSystem.FlushEvents();
            Logger.WriteToLog("trying to start single player");
            //this.optionsOnlySession.WriteXml(@"", GameXmlTypes.OptionsXml);
           // Logger.WriteToLog("User Aduio " + this.optionsOnlySession.options.UserAudioEnabled.ToString() + "User Bg " + this.optionsOnlySession.options.UserBackgroundsEnabled.ToString() + " Volume Level " + this.optionsOnlySession.options.VolumeLevel.ToString(), this);
            InterfaceUpdater.RemoveMainMenuHandlers(GameObject.Find("InterfaceUpdater").GetComponent<InterfaceUpdater>());
            Application.LoadLevel(1);
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.Log("Can't write options file");
            Debug.Log(e.Message);
            return;
#else
            Logger.WriteToLog("Can't start single player. Reason: " + e.Message + "\t At line " + Logger.GetExceptionsLineNumber(e), this);
            Application.Quit();
#endif
        }
    }

    public void QuitGame()
    {
        Globals.WriteAndCloseOptions();
        Logger.CloseLogFileHandle();
        Application.Quit();
    }

    public IEnumerator OptionsInterfaceUpdateRoutine()
    {
        InterfaceUpdateEventArgs e;
        object sender;
        Debug.Log("Coroutine started");

        e = new InterfaceUpdateEventArgs(InterfaceUpdateReasons.OptionChanged, "", Globals.options.UserAudioEnabled);//this.optionsOnlySession.options.UserAudioEnabled);
        
        EventSystem.FireInterfaceUpdate(GameObject.Find("AudioToggle"), e);
        //GameObject.Find("AudioToggle").GetComponent<Toggle>().isOn = Globals.options.UserAudioEnabled;

        //yield return new WaitForSeconds(0.2f);

        e = new InterfaceUpdateEventArgs(InterfaceUpdateReasons.OptionChanged, "", Globals.options.UserBackgroundsEnabled); //this.optionsOnlySession.options.UserBackgroundsEnabled);

        EventSystem.FireInterfaceUpdate(GameObject.Find("BackgroundToggle"), e);
        //GameObject.Find("BackgroundToggle").GetComponent<Toggle>().isOn = Globals.options.UserBackgroundsEnabled;


        //yield return new WaitForSeconds(0.2f);
        e = new InterfaceUpdateEventArgs(InterfaceUpdateReasons.OptionChanged, "", Globals.options.VolumeLevel); //this.optionsOnlySession.options.VolumeLevel);
        sender = GameObject.Find("VolumeSlider");

        GameObject.Find("Main Camera").GetComponent<AudioSource>().volume = Globals.options.VolumeLevel;//this.optionsOnlySession.options.VolumeLevel;
        
        EventSystem.FireInterfaceUpdate(sender, e);
        yield return null;
    }

    public void SetUserBackground()
    {
        Globals.options.UserBackgroundsEnabled = GameObject.Find("BackgroundToggle").GetComponent<Toggle>().isOn;
    }

    public void SetUserAudio()
    {
        Globals.options.UserAudioEnabled = GameObject.Find("AudioToggle").GetComponent<Toggle>().isOn;
    }

    public void SetVolumeLevel()
    {
        float val = GameObject.Find("VolumeSlider").GetComponent<Slider>().value;
        Globals.options.VolumeLevel = val;
        GameObject.Find("Main Camera").GetComponent<AudioSource>().volume = val;
    }

    /*private void OpenOptionsFile()
    {
        Logger.WriteToLog("What");
        this.optionsOnlySession = new GameSession(GameXmlTypes.OptionsXml);
        //Logger.WriteToLog("User Aduio " + this.optionsOnlySession.options.UserAudioEnabled.ToString() + "User Bg " + this.optionsOnlySession.options.UserBackgroundsEnabled.ToString() + " Volume Level " + this.optionsOnlySession.options.VolumeLevel.ToString(), this);
        StartCoroutine("OptionsInterfaceUpdateRoutine");
    }*/

    public void StartHotseat()
    {
        EventSystem.FlushEvents();
        Application.LoadLevel(2);
    }

    public void StartLan()
    { 
        
    }

    public void MultiplayerMods()
    {
        GameObject.Find("MultiplayerCanvas").GetComponent<Canvas>().enabled = true;
        GameObject.Find("MenuCanvas").GetComponent<Canvas>().enabled = false;
    }

    public void BackToMainMenu()
    {
        GameObject.Find("OptionsCanvas").GetComponent<Canvas>().enabled = false;
        GameObject.Find("MultiplayerCanvas").GetComponent<Canvas>().enabled = false;
        GameObject.Find("MenuCanvas").GetComponent<Canvas>().enabled = true;
        GameObject.Find("HeaderCanvas").GetComponent<Canvas>().enabled = true;
    }

    #region deprecation
    private static void OpenOptionsFile(MainMenuHelper instigator)
    {
        if (!File.Exists(instigator.optionsFilePath)) /* create standart options file with standart values*/
        {
            try
            {
                FileStream fs = File.Create(instigator.optionsFilePath);

                Type optionsType = typeof(Options);
                FieldInfo[] fi = optionsType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

                using (StreamWriter sw = new StreamWriter(fs))
                {
                    foreach (FieldInfo f in fi)
                    {
                        sw.WriteLine(f.Name + "=" + f.GetValue(instigator.options).ToString() + ";");
                    }
                }

                fs.Close();
            }
            catch (Exception e)
            {
#if UNITY_EDITOR
                Debug.Log("Error creating options file");
                Debug.Log(e.Message);
                return;
#else
                Application.Quit();
#endif
            }
        }
        else /* open existing file */
        {
            using (StreamReader sr = new StreamReader(File.Open(instigator.optionsFilePath, FileMode.Open, FileAccess.Read, FileShare.Read)))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    line = line.Replace(";", "");
                    string[] product = line.Split('=');
                    if (product.Length == 2)
                    {
                        try
                        {
                            instigator.options[product[0]] = product[1];
                        }
                        catch (Exception e)
                        {
#if UNITY_EDITOR
                            Debug.Log("Can't resolve property");
                            Debug.Log(e.Message);
                            return;
#else
                            Application.Quit();
#endif
                        }
                    }
                }
            }
        }

        instigator.StartCoroutine("OptionsInterfaceUpdateRoutine");
    }

    private static void WriteOptionsFile(MainMenuHelper instigator)
    {

    }
    #endregion

    void OnDestroy()
    {
        //Debug.LogError("Some crazy shit is going on here");
    }
}
