#if UNITY_EDITOR
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
    public GameObject[] secondPlayerBalls = null;
    public GameObject exceptionCanvasReference = null;
    public GameObject exceptionTextReference = null;
    public GameObject pauseMenuCanvasReference = null;
    public GameObject endGameCanvasReference = null;

    /*
     
     * TODO: add exception canvas and text
     * 
     */
    void Awake()
    {
        EventSystem.OnInterfaceUpdate += this.UpdateInterface;
    }

    void Start()
    {
        EventSystem.OnEndGame += this.ShowEndGameMenu;
    }

    void Update()
    {

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
}