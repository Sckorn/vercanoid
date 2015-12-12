using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;

public class Game{
    private bool gameInProgress = false;
    private int level = 0;
    private Field CurrentField;
    private Player HumanPlayer;
    private Player SecondHumanPlayer;
    private bool gamePaused = false;
    private GameModes currentGameMode;

    public GameModes CurrentGameMode
    {
        get { return this.currentGameMode; }
        set { this.currentGameMode = value; }
    }

    public bool GamePaused
    {
        get { return this.gamePaused; }
    }

    public int Level
    {
        get { return this.level; }
    }

    public bool GameInProgress
    {
        get { return this.gameInProgress; }
        set { this.gameInProgress = value; }
    }

    public Game()
    {
        Globals.SetGameMode(GameModes.SinglePlayer);
        this.gameInProgress = true;
        this.CurrentField = new Field(level);
        Logger.WriteToLog("Current level number " + level.ToString());
        this.HumanPlayer = new Player();
        EventSystem.OnEndLevel += this.ChangeLevel;
        EventSystem.OnEndGame += this.EndGame;
        Debug.Log("Game default constructor");
    }

    public Game(GameModes _gameMode)
    {
        Globals.SetGameMode(_gameMode);
        this.gameInProgress = true;
        this.CurrentField = new Field(level);
        this.HumanPlayer = new Player(Players.FirstPlayer);
        
        if(_gameMode == GameModes.Versus)
        {
            this.SecondHumanPlayer = new Player(Players.SecondPlayer);            
        }

        EventSystem.OnEndLevel += this.ChangeLevel;
        EventSystem.OnEndGame += this.EndGame;
        Debug.Log("Game constructor with parameter");
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

    public Player GetSecondHumanPlayer()
    {
        return this.SecondHumanPlayer;
    }

    public void UpdateUserScoreOnScreen() //deprecated
    {
        int score = this.HumanPlayer.GetLevelScore();

        GameObject.Find("LevelScoreText").GetComponent<Text>().text = score.ToString();
    }

    public void NextLevel()
    {
        ++this.level;
    }

    protected void ChangeLevel(object sender, ChangeLevelEventArgs e)
    {
        Debug.Log("Change level triggered");
        this.NextLevel();
        if (e.ChangeReason != ChangeLevelReasons.AllBricksDestroyed)
        {
            Debug.Log("Should destroy all bricks");
            this.CurrentField.DestroyAllBricks();
        }
        
        Debug.LogWarning(this.level.ToString() + " " + MainHelper.CurrentGameSession.CurrentLevels.TotalLevels.ToString());
        if (this.level == MainHelper.CurrentGameSession.CurrentLevels.TotalLevels)
        {
            EndGameEventArgs ea = new EndGameEventArgs(this.HumanPlayer.HighScore, this.HumanPlayer.PlayerName, this.level, EndGameReasons.CompletedAllLevels, this.DefineResult(EndGameReasons.CompletedAllLevels));
            EventSystem.FireEndGame(this, ea);
        }
        else
            this.CurrentField = new Field(this.level);
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        this.gamePaused = true;
        object sender = new object();
        InterfaceUpdateEventArgs e = new InterfaceUpdateEventArgs(InterfaceUpdateReasons.GamePaused, "", null, this.GamePaused);
        EventSystem.FireInterfaceUpdate(sender, e);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        this.gamePaused = false;
        object sender = new object();
        InterfaceUpdateEventArgs e = new InterfaceUpdateEventArgs(InterfaceUpdateReasons.GamePaused, "", null, this.GamePaused);
        EventSystem.FireInterfaceUpdate(sender, e);
    }

    public void EndGame(object sender, EndGameEventArgs e)
    {
        this.gameInProgress = false;
        this.level = 0;
    }

    public string DefineResult(EndGameReasons reason)
    {
        int firstPlayerPoints;
        int secondPlayerPoints;
        int firstPlayerBalls;
        int SecondPlayerBalls;
        try
        {
            firstPlayerPoints = this.HumanPlayer.HighScore;
            secondPlayerPoints = this.SecondHumanPlayer.HighScore;
            firstPlayerBalls = this.HumanPlayer.CurrentBall;
            SecondPlayerBalls = this.SecondHumanPlayer.CurrentBall;
        }
        catch (Exception ex)
        { 
#if UNITY_EDITOR
            Debug.Log("Null player reference");
            Debug.Log(ex.Message);
            return string.Empty;
#else
            InterfaceUpdateEventArgs iuea = new InterfaceUpdateEventArgs(InterfaceUpdateReasons.ExceptionThrown, "Null player reference", ex);
            EventSystem.FireInterfaceUpdate(this, iuea); 
            return string.Empty;
#endif
        }

        int difference = firstPlayerPoints - secondPlayerPoints;

        string resultString = string.Empty;

        if (reason == EndGameReasons.CompletedAllLevels)
        {
            if (difference == 0)
            {
                resultString = "Draw";
            }
            else
                if (difference < 0)
                {
                    resultString = "Second Player Wins";
                }
                else
                {
                    resultString = "First Player Wins";
                }
        }
        else if (reason == EndGameReasons.WastedAllBalls)
        {
            if (firstPlayerBalls > SecondPlayerBalls)
            {
                resultString = "Second Player Wins";
            }
            else
            {
                resultString = "First Player Wins";
            }
        }

        //InterfaceUpdateEventArgs iuea = new InterfaceUpdateEventArgs(InterfaceUpdateReasons.
        return resultString;
    }

    public void RemoveDelegates()
    {
        EventSystem.OnEndLevel -= this.ChangeLevel;
        EventSystem.OnEndGame -= this.EndGame;
    }
}
