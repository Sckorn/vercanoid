using UnityEngine;
using System.Collections;

public enum ChangeLevelReasons {

    AllBricksDestroyed,
#if UNITY_EDITOR
    NextLevelKeyPressed,
#endif
    UnknownReason

}
