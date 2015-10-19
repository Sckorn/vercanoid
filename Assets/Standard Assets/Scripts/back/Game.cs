using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Game{
    private bool gameInProgress = false;
    private int level = 0;
    private Field CurrentField;
    private Player HumanPlayer;
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
        this.gameInProgress = true;
        this.CurrentField = new Field(level);
        this.HumanPlayer = new Player();
        EventSystem.OnEndLevel += this.ChangeLevel;
    }

    public Game(GameModes _gameMode)
    {
        this.gameInProgress = true;
        this.CurrentField = new Field(level);
        this.HumanPlayer = new Player(Players.FirstPlayer);
        
        if(_gameMode == GameModes.Versus)
        {
            this.HumanPlayer = new Player(Players.SecondPlayer);
        }

        EventSystem.OnEndLevel += this.ChangeLevel;
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
        this.NextLevel();
        if(e.ChangeReason != ChangeLevelReasons.AllBricksDestroyed)
            this.CurrentField.DestroyAllBricks();
        this.CurrentField = new Field(this.level);
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        this.gamePaused = true;
        object sender = new object();
        InterfaceUpdateEventArgs e = new InterfaceUpdateEventArgs(InterfaceUpdateReasons.GamePaused, "", this.GamePaused);
        EventSystem.FireInterfaceUpdate(sender, e);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        this.gamePaused = false;
        object sender = new object();
        InterfaceUpdateEventArgs e = new InterfaceUpdateEventArgs(InterfaceUpdateReasons.GamePaused, "", this.GamePaused);
        EventSystem.FireInterfaceUpdate(sender, e);
    }
}
