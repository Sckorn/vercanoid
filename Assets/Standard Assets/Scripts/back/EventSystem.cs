using UnityEngine;
using System;
using System.Collections;

public class EventSystem : MonoBehaviour {
    private MainHelper mhReference;

    public delegate void EndLevelAction();
    public static event EndLevelAction OnEndLevel;

    public delegate void EndGameAction(object sender, EndGameEventArgs e);
    public static event EndGameAction OnEndGame;

    public delegate void BallCrushedAction(object sender, BallCrushedEventArgs e);
    public static event BallCrushedAction OnBallCrush;
	// Use this for initialization
	void Start () {
        this.mhReference = GameObject.Find("MainHelper").GetComponent<MainHelper>();
	}
	
	// Update is called once per frame
	void Update () {
        if (this.mhReference.GetCurrentGame().GetCurrentField().TotalBricks == 0)
            EventSystem.FireEndLevel();
	}

    public static void FireEndLevel()
    {
        if (EventSystem.OnEndLevel != null)
        {
            EventSystem.OnEndLevel();
        }
    }

    public static void FireEndGame(object sender, EndGameEventArgs e)
    {
        if (EventSystem.OnEndGame != null)
        {
            EventSystem.OnEndGame(sender, e);
        }
    }

    public static void FireBallCrush(object sender, BallCrushedEventArgs e)
    {
        if (EventSystem.OnBallCrush != null)
        {
            EventSystem.OnBallCrush(sender, e);
        }
    }
}
