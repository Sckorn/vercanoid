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

    public BallCrushedEventArgs(BallCrushReasons crushReason, int currentBall)
    {
        this.crushReason = crushReason;
        this.currentBall = currentBall;
    }
}
