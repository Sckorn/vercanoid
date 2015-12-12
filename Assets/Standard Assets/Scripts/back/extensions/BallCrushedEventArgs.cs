using UnityEngine;
using System;
using System.Collections;

public class BallCrushedEventArgs : EventArgs {

    private BallCrushReasons crushReason;

    public BallCrushReasons CrushReasons
    {
        get { return this.crushReason; }
    }

    private int currentBall;

    public int CurrentBall
    {
        get { return this.currentBall; }
    }

    private Players fromPlayer;

    public Players FromPlayer
    {
        get { return this.fromPlayer; }
    }

    public BallCrushedEventArgs(BallCrushReasons crushReason, int currentBall)
    {
        this.crushReason = crushReason;
        this.currentBall = currentBall;
    }

    public BallCrushedEventArgs(BallCrushReasons crushReason, int currentBall, Players player)
    {
        this.crushReason = crushReason;
        this.currentBall = currentBall;
        this.fromPlayer = player;
    }
}
