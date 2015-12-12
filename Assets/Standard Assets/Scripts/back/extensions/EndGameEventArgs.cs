using UnityEngine;
using System;
using System.Collections;

public class EndGameEventArgs : EventArgs {

    /*
     
     * TODO: use later for end gme event
     
     */

    private int totalScore;

    public int TotalScore
    {
        get { return this.totalScore; }
    }

    private string playerName;

    public string PlayerName
    {
        get { return this.playerName; }
    }

    private int endGameLevel;

    public int EndGameLevel
    {
        get { return this.endGameLevel; }
    }

    private EndGameReasons endReason;

    public EndGameReasons EndReason
    {
        get { return this.endReason; }
    }

    private string gameResult;

    public string GameResult
    {
        get { return this.gameResult; }
    }

    public EndGameEventArgs(int totalScore, string playerName, int endGameLevel)
    {
        this.totalScore = totalScore;
        this.playerName = playerName;
        this.endGameLevel = endGameLevel;
        this.endReason = EndGameReasons.UnknownReason;
    }

    public EndGameEventArgs(int totalScore, string playerName, int endGameLevel, EndGameReasons endReason)
    {
        this.totalScore = totalScore;
        this.playerName = playerName;
        this.endGameLevel = endGameLevel;
        this.endReason = endReason;
    }

    public EndGameEventArgs(int totalScore, string playerName, int endGameLevel, EndGameReasons endReason, string result)
    {
        this.totalScore = totalScore;
        this.playerName = playerName;
        this.endGameLevel = endGameLevel;
        this.endReason = endReason;
        this.gameResult = result;
    }
	
}
