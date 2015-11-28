﻿#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class InterfaceUpdater : MonoBehaviour
{

    public GameObject firstPlayerScore = null;
    public GameObject secondPlayerScore = null;
    public GameObjectArrayLayout firstPlayerBalls;
    public GameObjectArrayLayout secondPlayerBalls;
    public GameObject exceptionCanvasReference = null;
    public GameObject exceptionTextReference = null;
    public GameObject pauseMenuCanvasReference = null;
    public GameObject endGameCanvasReference = null;
    public GameObject headerCanvasReference = null;
    public GameObject menuCanvasReference = null;
    public GameObject optionsCanvasReference = null;
    public GameObject backgroundToggleReference = null;
    public GameObject audioToggleReference = null;
    public GameObject volumeLevelSliderReference = null;

    /*
     
     * TODO: add exception canvas and text
     * 
     */
    void Awake()
    {
        //Debug.LogError("Interface Updater awakes");
        EventSystem.OnInterfaceUpdate += this.UpdateInterface;
        EventSystem.OnInterfaceUpdate += this.UpdateAudioToggle;
        EventSystem.OnInterfaceUpdate += this.UpdateBackgroundToggle;
        if (this.volumeLevelSliderReference != null)
        {
            EventSystem.OnInterfaceUpdate += this.UpdateVolumeSlider;
        }
        //Debug.Log(Globals.options.UserAudioEnabled.ToString() + " " + Globals.options.UserBackgroundsEnabled.ToString() + " " + Globals.options.VolumeLevel.ToString());
    }

    void Start()
    {
        EventSystem.OnEndGame += this.ShowEndGameMenu;
        /*EventSystem.OnInterfaceUpdate += this.UpdateInterface;
        EventSystem.OnInterfaceUpdate += this.UpdateAudioToggle;
        EventSystem.OnInterfaceUpdate += this.UpdateBackgroundToggle;
        EventSystem.OnInterfaceUpdate += this.UpdateVolumeSlider;        */
        //this.volumeLevelSliderReference = GameObject.Find("VolumeSlider");
    }

    void Update()
    {

    }

    void OnDestroy()
    {
        EventSystem.OnInterfaceUpdate -= this.UpdateInterface;
        EventSystem.OnInterfaceUpdate -= this.UpdateAudioToggle;
        EventSystem.OnInterfaceUpdate -= this.UpdateBackgroundToggle;
        EventSystem.OnInterfaceUpdate -= this.UpdateVolumeSlider;
        EventSystem.OnEndGame -= this.ShowEndGameMenu;
    }

    protected void UpdateInterface(object sender, InterfaceUpdateEventArgs e)
    {
        switch (e.UpdateReason)
        {
            case InterfaceUpdateReasons.BallLost:
                Player ballLostSender = (Player)sender;

                if (ballLostSender.PlayerScoreGUI.Equals(this.firstPlayerScore))
                {
                    int index = ballLostSender.CurrentBall;
                    GameObject target = this.firstPlayerBalls.objectReferences[index - 2]; //don't ask, just don't...
                    if(target != null) // balls start at one [1]
                    {
                        GameObject.Find("Morpher").GetComponent<Morpher>().KillBallIndicator(target);
                        target = null;
                    }
                }
                else
                { 
                    /* enemy ball remove handling*/
                }

                break;
            case InterfaceUpdateReasons.LevelChanged:
                if (this.firstPlayerScore != null)
                    this.firstPlayerScore.GetComponent<Text>().text = (0).ToString();
                if (this.secondPlayerScore != null)
                    this.secondPlayerScore.GetComponent<Text>().text = (0).ToString();
                break;
            case InterfaceUpdateReasons.ScoreIncreased:
                Player scoreIncreasedSender = (Player)sender;

                if (scoreIncreasedSender.PlayerScoreGUI.Equals(this.firstPlayerScore))
                {
                    this.firstPlayerScore.GetComponent<Text>().text = e.UpdatedValue;
                }
                else
                {
                    /*second player score handling*/
                }
                break;
            case InterfaceUpdateReasons.GameOver: break;
            case InterfaceUpdateReasons.ExceptionThrown:
                if(e.ExceptionUpdate)
                    this.ShowException(e);
                break;
            case InterfaceUpdateReasons.GamePaused:
                this.ShowPauseMenu(e);
                break;
            case InterfaceUpdateReasons.UnknownReason: break;
        }
    }


    protected void ShowException(InterfaceUpdateEventArgs e)
    {
        Time.timeScale = 0;
        this.exceptionCanvasReference.GetComponent<Canvas>().enabled = true;
        this.exceptionTextReference.GetComponent<Text>().text += " " + e.UpdatedValue;
    }

    public void ResumeButtonClickHandler()
    {
        MainHelper mh = GameObject.Find("MainHelper").GetComponent<MainHelper>();
        mh.GetCurrentGame().ResumeGame();
    }

    public void ToMainMenuClickHandler()
    {
        EventSystem.FlushEvents();
        Application.LoadLevel(0);
    }

    protected void ShowPauseMenu(InterfaceUpdateEventArgs e)
    {
        if (e.GamePaused)
        {
            GameObject.Find("PauseGameCanvas").GetComponent<Canvas>().enabled = true;
        }
        else
        {
            GameObject.Find("PauseGameCanvas").GetComponent<Canvas>().enabled = false;
        }
    }

    protected void ShowEndGameMenu(object sender, EndGameEventArgs e)
    {
        if (this.endGameCanvasReference != null)
        {
            this.endGameCanvasReference.GetComponent<Canvas>().enabled = true;
        }
    }

    protected void UpdateBackgroundToggle(object sender, InterfaceUpdateEventArgs e)
    {
        GameObject goSender;
        try
        {
            goSender = (GameObject)sender;
        }
        catch (Exception exc)
        {
            Debug.Log("Sender is null or of a wrong type");
            Debug.Log(exc.Message);
            //EventSystem.OnInterfaceUpdate -= this.UpdateBackgroundToggle;
            return;
        }

        if (e.UpdateReason == InterfaceUpdateReasons.OptionChanged)
        {
            if (this.backgroundToggleReference != null)
            {
                if (goSender.Equals(this.backgroundToggleReference.gameObject))
                {
                    try
                    {
                        bool value = (bool)e.TargetValue;
                        this.backgroundToggleReference.GetComponent<Toggle>().isOn = value;
                    }
                    catch (Exception ex)
                    {
                        Debug.Log("Can't update interface");
                        Debug.Log(ex.Message);
                        return;
                    }
                }
            }
        }
    }

    protected void UpdateAudioToggle(object sender, InterfaceUpdateEventArgs e)
    {
        GameObject goSender;
        try
        {
            goSender = (GameObject)sender;
        }
        catch (Exception exc)
        {
            Debug.Log("Sender is null or of a wrong type");
            Debug.Log(exc.Message);
            //EventSystem.OnInterfaceUpdate -= this.UpdateAudioToggle;
            return;
        }
        //Debug.Log(goSender.ToString());
        if (e.UpdateReason == InterfaceUpdateReasons.OptionChanged)
        {
            if (this.audioToggleReference != null)
            {
                if (goSender.Equals(this.audioToggleReference.gameObject))
                {
                    try
                    {
                        bool value = (bool)e.TargetValue;
                        this.audioToggleReference.GetComponent<Toggle>().isOn = value;
                    }
                    catch (Exception ex)
                    {
                        Debug.Log("Can't update interface");
                        Debug.Log(ex.Message);
                        return;
                    }
                }
            }
        }
    }

    protected void UpdateVolumeSlider(object sender, InterfaceUpdateEventArgs e)
    {
        Debug.Log("Updating volume");
        GameObject goSender;
        try
        {
            goSender = (GameObject)sender;
        }
        catch (Exception exc)
        {
            Debug.Log("Sender is null or of a wrong type");
            Debug.Log(exc.Message);
            //EventSystem.OnInterfaceUpdate -= this.UpdateVolumeSlider;
            return;
        }

        if (e.UpdateReason == InterfaceUpdateReasons.OptionChanged)
        {
            Debug.LogError("Option changed");
            if (this.volumeLevelSliderReference != null)
            {
                Debug.LogError("Slider reference?");
                Debug.LogError("GameObject " + (this.volumeLevelSliderReference.gameObject == null).ToString());
                Debug.LogError("goSender " + (goSender == null).ToString());
                Debug.LogError("Equality between them " + (goSender == this.volumeLevelSliderReference.gameObject).ToString());
                Debug.LogError("Built in reference " + this.volumeLevelSliderReference.ToString());
                Debug.LogError("Sender reference " + goSender.ToString());
                if (goSender.Equals(this.volumeLevelSliderReference.gameObject))
                {
                    Debug.LogError("Equals?");
                    float tmp;
                    try
                    {
                        tmp = (float)e.TargetValue;
                        this.volumeLevelSliderReference.GetComponent<Slider>().value = tmp;
                    }
                    catch (Exception ex)
                    {
                        Debug.Log("Can't update interface");
                        Debug.Log(ex.Message);
                        return;
                    }
                }
            }
        }
    }

    public void ShowOptionsCanvas()
    {
        try
        {
            this.headerCanvasReference.GetComponent<Canvas>().enabled = false;
            this.menuCanvasReference.GetComponent<Canvas>().enabled = false;
            this.optionsCanvasReference.GetComponent<Canvas>().enabled = true;
        }
        catch (Exception e)
        {
            try
            {
                GameObject.Find("HeaderCanvas").GetComponent<Canvas>().enabled = false;
                GameObject.Find("MenuCanvas").GetComponent<Canvas>().enabled = false;
                GameObject.Find("OptionsCanvas").GetComponent<Canvas>().enabled = true;
            }
            catch (Exception ex)
            {
#if UNITY_EDITOR
                Debug.Log("No canvas references");
                Debug.Log(ex.Message);
                Debug.Log(e.Message);
#else
                Logger.WriteToLog("No canvas references. " + e.Message + "; " + ex.Message + "\t At line " + Logger.GetExceptionsLineNumber(e).ToString() + "\t And at line: " + Logger.GetExceptionsLineNumber(ex).ToString(), this);
#endif
            }
        }
    }

    public static void RemoveMainMenuHandlers(InterfaceUpdater instigator)
    {
        EventSystem.OnInterfaceUpdate -= instigator.UpdateAudioToggle;
        EventSystem.OnInterfaceUpdate -= instigator.UpdateBackgroundToggle;
        EventSystem.OnInterfaceUpdate -= instigator.UpdateVolumeSlider;
        EventSystem.OnInterfaceUpdate -= instigator.UpdateInterface;
    }

    public void ChangeColorsByTagsHotseat()
    {
        GameObject[] firstPlayerUiElements = GameObject.FindGameObjectsWithTag("FirstPlayerUI");
        Debug.Log(firstPlayerUiElements.Length.ToString());
        foreach (GameObject go in firstPlayerUiElements)
        {
            if(go.GetComponent<Text>() != null)
                go.GetComponent<Text>().color = Color.red;

            if (go.GetComponent<Image>() != null)
                go.GetComponent<Image>().color = Color.red;            
        }

        GameObject[] secondPlayerUiElements = GameObject.FindGameObjectsWithTag("SecondPlayerUI");

        foreach (GameObject go in secondPlayerUiElements)
        {
            if (go.GetComponent<Text>() != null)
                go.GetComponent<Text>().color = Color.blue;

            if (go.GetComponent<Image>() != null)
                go.GetComponent<Image>().color = Color.blue;
        }
    }
}