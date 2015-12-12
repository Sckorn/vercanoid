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
        //if(Globals.CurrentGameMode == GameModes.SinglePlayer)
            this.mhReference = GameObject.Find("MainHelper").GetComponent<MainHelper>();
	}
	
	// Update is called once per frame
	void Update () {
        if (GameObject.Find("MainHelper").GetComponent<MainHelper>().GetCurrentGame().GameInProgress)
        {
            if (this.mhReference.GetCurrentGame().GetCurrentField().TotalBricks == 1)
            {
                Debug.LogWarning("Change level fired");
                object sender = new object();
                ChangeLevelEventArgs e = new ChangeLevelEventArgs(this.mhReference.GetCurrentGame().Level, this.mhReference.GetCurrentGame().Level + 1, ChangeLevelReasons.AllBricksDestroyed);
                EventSystem.FireChangeLevel(sender, e);
            }
        }
	}

    public static void FireChangeLevel(object sender, ChangeLevelEventArgs e)
    {
        if (EventSystem.OnEndLevel != null)
        {
            EventSystem.OnEndLevel(sender, e);
            foreach (Delegate del in EventSystem.OnEndLevel.GetInvocationList())
            {
                Debug.LogWarning(del.Method.Name.ToString());
                Debug.LogWarning(del.Target.ToString());
            }
            InterfaceUpdateEventArgs ev = new InterfaceUpdateEventArgs(InterfaceUpdateReasons.LevelChanged, "");
            EventSystem.FireInterfaceUpdate(sender, ev);
        }
    }

    public static void FireEndGame(object sender, EndGameEventArgs e)
    {
        if (GameObject.Find("MainHelper").GetComponent<MainHelper>().GetCurrentGame().GameInProgress)
        {
            if (EventSystem.OnEndGame != null)
            {
                /*Debug.LogWarning("End Game fired");
                foreach (Delegate del in EventSystem.OnEndGame.GetInvocationList())
                {
                    Debug.LogWarning(del.Method.Name.ToString());
                    Debug.LogWarning(del.Target.ToString());
                }*/

                EventSystem.OnEndGame(sender, e);
            }
        }
    }

    public static void FireBallCrush(object sender, BallCrushedEventArgs e)
    {
        if (EventSystem.OnBallCrush != null)
        {
            foreach(Delegate del in EventSystem.OnBallCrush.GetInvocationList())
            {
                Debug.LogWarning(del.Method.Name);
                Debug.LogWarning(del.Target.ToString());
            }
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

    public static void FlushEvents()
    {
        EventSystem.OnBallCrush = null;
        EventSystem.OnEndGame = null;
        EventSystem.OnEndLevel = null;
        EventSystem.OnInterfaceUpdate = null;
    }
}
