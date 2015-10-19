using UnityEngine;
using System.Collections;

public class DelayedCollision {

    public ContactPoint cp;
    public bool platformFlag;

    public DelayedCollision()
    { 
        
    }

    public DelayedCollision(ContactPoint _cp, bool _platformFlag)
    {
        this.cp = _cp;
        this.platformFlag = _platformFlag;
    }
}
