using UnityEngine;
using System;
using System.Collections;

public class ChangeLevelEventArgs : EventArgs {

    private ChangeLevelReasons changeReason;

    public ChangeLevelReasons ChangeReason
    {
        get { return this.changeReason; }
    }

    private int finishedLevel;

    public int FinishedLevel
    {
        get { return this.finishedLevel; }
    }

    private int nextLevel;

    public int NextLevel
    {
        get { return this.nextLevel; }
    }

    public ChangeLevelEventArgs(int finishedLevel, int nextLevel, ChangeLevelReasons changeReason)
    {
        this.finishedLevel = finishedLevel;
        this.nextLevel = nextLevel;
        this.changeReason = changeReason;
    }
}
