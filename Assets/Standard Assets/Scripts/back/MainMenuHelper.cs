using UnityEngine;
using UnityEngine.UI;
using System;
using System.Reflection;
using System.IO;
using System.Collections;
using SimpleFileCrypter;

public class MainMenuHelper : MonoBehaviour {
    private string optionsFilePath = @"Data/Options.ini";
    private Options options = new Options();
    private GameSession optionsOnlySession;

    /*[System.Runtime.InteropServices.DllImport(@"SimpleFileCrypter.dll")]
    static public extern void EncryptFileFromString(string sInputString, string sOutputFilePath, string sKey);*/

	// Use this for initialization
	void Start () {
        //MainMenuHelper.OpenOptionsFile(this);
        this.ResizeHeader();
        this.OpenOptionsFile();
        //MainMenuHelper.EncryptFileFromString("fuck you twice", @"Temp/dafuq.txt", "");
        LevelFileCrypto.DecryptFile(@"1.xml", "Temp/1.xml", "");
        //Logger.CreateLogFile();
        Logger.RemoveAllExistingLogs();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ResizeHeader()
    {
        RectTransform rt = GameObject.Find("HeaderPanel").GetComponentInChildren<RectTransform>();
        //Debug.Log(rt.sizeDelta.x.ToString() + " " + rt.sizeDelta.y.ToString());
        float quarterOfHeight = Screen.height / 4;
        rt.sizeDelta = new Vector2(Screen.width, quarterOfHeight);
    }

    public void StartSinglePlayer()
    {
        try
        {
            this.optionsOnlySession.WriteXml(@"Data/Options.xml", GameXmlTypes.OptionsXml);
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
            Application.Quit();
#endif
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public IEnumerator OptionsInterfaceUpdateRoutine()
    {
        Debug.Log("Coroutine");
        InterfaceUpdateEventArgs e;
        object sender;

        /*e = new InterfaceUpdateEventArgs(InterfaceUpdateReasons.OptionChanged, "", this.options.UserAudioEnabled);

        EventSystem.FireInterfaceUpdate(GameObject.Find("AudioToggle"), e);

        yield return new WaitForSeconds(0.2f);

        e = new InterfaceUpdateEventArgs(InterfaceUpdateReasons.OptionChanged, "",  this.options.UserBackgroundsEnabled);

        EventSystem.FireInterfaceUpdate(GameObject.Find("BackgroundToggle"), e);

        yield return new WaitForSeconds(0.2f);

        e = new InterfaceUpdateEventArgs(InterfaceUpdateReasons.OptionChanged, "", this.options.VolumeLevel);
        sender = GameObject.Find("VolumeSlider");
        GameObject.Find("Main Camera").GetComponent<AudioSource>().volume = this.options.VolumeLevel;

        EventSystem.FireInterfaceUpdate(sender, e);*/

        e = new InterfaceUpdateEventArgs(InterfaceUpdateReasons.OptionChanged, "", this.optionsOnlySession.options.UserAudioEnabled);

        EventSystem.FireInterfaceUpdate(GameObject.Find("AudioToggle"), e);

        yield return new WaitForSeconds(0.2f);

        e = new InterfaceUpdateEventArgs(InterfaceUpdateReasons.OptionChanged, "", this.optionsOnlySession.options.UserBackgroundsEnabled);

        EventSystem.FireInterfaceUpdate(GameObject.Find("BackgroundToggle"), e);

        yield return new WaitForSeconds(0.2f);

        e = new InterfaceUpdateEventArgs(InterfaceUpdateReasons.OptionChanged, "", this.optionsOnlySession.options.VolumeLevel);
        sender = GameObject.Find("VolumeSlider");
        GameObject.Find("Main Camera").GetComponent<AudioSource>().volume = this.optionsOnlySession.options.VolumeLevel;

        EventSystem.FireInterfaceUpdate(sender, e);
    }

    public void SetUserBackground()
    {
        this.options.UserBackgroundsEnabled = GameObject.Find("BackgroundToggle").GetComponent<Toggle>().isOn;
    }

    public void SetUserAudio()
    {
        this.options.UserAudioEnabled = GameObject.Find("AudioToggle").GetComponent<Toggle>().isOn;
    }

    public void SetVolumeLevel()
    {
        float val = GameObject.Find("VolumeSlider").GetComponent<Slider>().value;
        this.optionsOnlySession.options.VolumeLevel = val;
        GameObject.Find("Main Camera").GetComponent<AudioSource>().volume = val;
    }

    private void OpenOptionsFile()
    {
        this.optionsOnlySession = new GameSession(GameXmlTypes.OptionsXml);

        StartCoroutine("OptionsInterfaceUpdateRoutine");
    }

    public void StartHotseat()
    {
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
}
