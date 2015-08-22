﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player {
    private string playerName = "Player1";
    private List<int> scorePerLevel;
    private int highScore = 0;
    private int currentLevel = 0;
    private int totalLevels;
    private int totalBalls = 3;
    private int currentBall = 1;
    private GameObject playerScoreInterface;

    public int TotalLevels
    {
        get { return this.totalLevels; }
        set { this.totalLevels = value; }
    }

    public int CurrentLevel
    {
        get { return this.currentLevel; }
        set { this.currentLevel = value; }
    }

    public string PlayerName
    {
        get { return this.playerName; }
        set { this.playerName = value; }
    }

    public int HighScore
    {
        get { return this.highScore; }
    }

    public GameObject PlayerScoreGUI
    {
        get { return this.playerScoreInterface; }
    }

    public Player()
    {
        this.scorePerLevel = new List<int>();
        this.scorePerLevel.Add(0);
        EventSystem.OnEndLevel += this.NextLevel;
        EventSystem.OnEndGame += this.EndGame;
        EventSystem.OnBallCrush += this.IncCurrentBall;
    }

    public Player(GameObject guiReference)
    {
        this.scorePerLevel = new List<int>();
        this.scorePerLevel.Add(0);
        this.playerScoreInterface = guiReference;
        EventSystem.OnEndLevel += this.NextLevel;
        EventSystem.OnEndGame += this.EndGame;
        EventSystem.OnBallCrush += this.IncCurrentBall;
    }

    public void NextLevel(object sender, ChangeLevelEventArgs ea)
    {
        if ((this.currentLevel + 1) >= this.totalLevels) return;

        try
        {
            this.AddToHighScore(this.scorePerLevel[this.currentLevel]);
        }
        catch (KeyNotFoundException e)
        {
#if UNITY_EDITOR
            Debug.LogError("No such index.");
            Debug.LogError(e.Message);
            return;
#else
            Application.Quit();
#endif
        }
        finally
        {
            this.CurrentLevel += 1;
        }

        try
        {
            if (!this.scorePerLevel.Exists(x => x == this.CurrentLevel))
                this.scorePerLevel.Add(0);
        }
        catch (KeyNotFoundException e)
        {
#if UNITY_EDITOR
            Debug.LogError("No such index.");
            Debug.LogError(e.Message);
            return;
#else
            Application.Quit();
#endif
        }
    }

    private void AddToHighScore(int score)
    {
        this.highScore += score;
    }

    public void IncCurrentBall(object sender, BallCrushedEventArgs e)
    {
        /*TODO: research on invoking using reflection, probably it will be much more efficient to use enums to invoke methods with special naming rules using parts of enum [On<EnumName>Action(object, EventArgs)]*/
        switch (e.CrushReasons)
        {
            case BallCrushReasons.EnenmyBackWallCrush: break; // vs mode
            case BallCrushReasons.PlayerBackWallCrush:
                this.IncBallNum(sender, e);
                break;
            case BallCrushReasons.UnknownReason: break;
        }
    }

    protected void IncBallNum(object sender, BallCrushedEventArgs e)
    {
        Debug.Log("Action called, balls: " + this.currentBall.ToString() + " total: " + this.totalBalls.ToString());
        if (this.currentBall <= this.totalBalls)
        {
            ++this.currentBall;
        }
        else
        {
            object snd = new object();
            EndGameEventArgs ev = new EndGameEventArgs(this.highScore, this.playerName, this.currentLevel, EndGameReasons.WastedAllBalls);
            EventSystem.FireEndGame(snd, ev); // TODO: eventsystem game end event 
        }
    }

    protected void EndGame(object sender, EndGameEventArgs e)
    {
        switch (e.EndReason)
        {
            case EndGameReasons.CompletedAllLevels:
                    
                break;
            case EndGameReasons.WastedAllBalls:
                this.WastedAllBallsEnd(sender, e);
                break;
            case EndGameReasons.UnknownReason: 
                    
                break;
        }
    }

    protected void WastedAllBallsEnd(object sender, EndGameEventArgs e)
    {
        Time.timeScale = 0;
        MainHelper mh = GameObject.Find("MainHelper").GetComponent<MainHelper>();
        Game currentGame = mh.GetCurrentGame();
        currentGame.GameInProgress = false;

        GameObject ballObj = null;

        ballObj = GameObject.Find("Ball");

        //if (ballObj != null)
        //{
            GameObject.Find("Morpher").GetComponent<Morpher>().KillTheBall();
        //}
    }

    public int GetLevelScore(int levelNum = -1)
    {
        if (levelNum < 0)
            levelNum = this.currentLevel;

        int result = 0;

        try
        {
            result = this.scorePerLevel[levelNum];
        }
        catch (KeyNotFoundException e)
        {
#if UNITY_EDITOR
            Debug.LogError("No such index.");
            Debug.LogError(e.Message);
            return -1;
#else
            Application.Quit();
#endif
        }

        return result;
    }

    public void IncreaseLevelScore(int score, int levelNum = -1)
    {
        if (levelNum < 0)
            levelNum = this.currentLevel;

        try
        {
            this.scorePerLevel[levelNum] += score;
        }
        catch (KeyNotFoundException e)
        { 
#if UNITY_EDITOR
            Debug.LogError("No such index.");
            Debug.LogError(e.Message);
            return;
#else
            Application.Quit();
#endif
        }
    }
}
