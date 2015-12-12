using UnityEngine;
using System.Collections;

public enum BallCrushReasons
{ 
    PlayerBackWallCrush, // single game mode - first player crush
    EnenmyBackWallCrush, // versus game mode - second player crush
    UnknownReason //some extraordinary shit happened
}
