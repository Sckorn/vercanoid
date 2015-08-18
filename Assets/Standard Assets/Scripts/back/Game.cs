﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Game{
    private bool gameInProgress = false;
    private int level = 0;
    private Field CurrentField;
    private Player HumanPlayer;

    public bool GameInProgress
    {
        get { return this.gameInProgress; }
        set { this.gameInProgress = value; }
    }

    public Game()
    {
        this.gameInProgress = true;
        this.CurrentField = new Field(level);
        this.HumanPlayer = new Player();
    }

    public bool LevelIsOver()
    {
        return true;
    }

    public Field GetCurrentField()
    {
        return this.CurrentField;
    }

    public Player GetHumanPlayer()
    {
        return this.HumanPlayer;
    }

    public void UpdateUserScoreOnScreen()
    {
        int score = this.HumanPlayer.GetLevelScore();

        GameObject.Find("LevelScoreText").GetComponent<Text>().text = score.ToString();
    }
}
