using UnityEngine;
using System;
using System.Collections;

public class EventSystem : MonoBehaviour {
    private MainHelper mhReference;

    public delegate void ChangeLevelAction(object sender, ChangeLevelEventArgs e);
    public static event ChangeLevelAction OnEndLevel;

    public delegate void EndGameAction(object sender, EndGameEventArgs e);
    public static event EndGameAction OnEndGame;

    public delegate void BallCrushedAction(object sender, BallCrushedEventArgs e);
    public static event BallCrushedAction OnBallCrush;

    public delegate void InterfaceUpdateAction(object sender, InterfaceUpdateEventArgs e);
    public static event InterfaceUpdateAction OnInterfaceUpdate;

	// Use this for initialization
	void Start () {
        this.mhReference = GameObject.Find("MainHelper").GetComponent<MainHelper>();
	}
	
	// Update is called once per frame
	void Update () {
        if (this.mhReference.GetCurrentGame().GetCurrentField().TotalBricks == 0)
        {
            object sender = new object();
            ChangeLevelEventArgs e = new ChangeLevelEventArgs(this.mhReference.GetCurrentGame().Level, this.mhReference.GetCurrentGame().Level + 1, ChangeLevelReasons.AllBricksDestroyed);
            EventSystem.FireChangeLevel(sender, e);
        }
	}

    public static void FireChangeLevel(object sender, ChangeLevelEventArgs e)
    {
        if (EventSystem.OnEndLevel != null)
        {
            EventSystem.OnEndLevel(sender, e);
            InterfaceUpdateEventArgs ev = new InterfaceUpdateEventArgs(InterfaceUpdateReasons.LevelChanged, "");
            EventSystem.FireInterfaceUpdate(sender, ev);
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

    public static void FireInterfaceUpdate(object sender, InterfaceUpdateEventArgs e)
    {
        if (EventSystem.OnInterfaceUpdate != null)
        {
            EventSystem.OnInterfaceUpdate(sender, e);
        }
    }
}
