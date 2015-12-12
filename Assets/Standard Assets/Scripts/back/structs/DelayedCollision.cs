using UnityEngine;
using System.Collections;

public class DelayedCollision {

    public ContactPoint cp;
    public bool platformFlag;
    public GameObject sender;
    public Players playerOwner;

    public DelayedCollision()
    { 
        
    }

    public DelayedCollision(ContactPoint _cp, bool _platformFlag)
    {
        this.cp = _cp;
        this.platformFlag = _platformFlag;
        this.sender = null;
    }

    public DelayedCollision(ContactPoint _cp, bool _platformFlag, GameObject _sender)
    {
        this.cp = _cp;
        this.platformFlag = _platformFlag;
        this.sender = _sender;
    }

    public DelayedCollision(ContactPoint _cp, bool _platformFlag, GameObject _sender, Players _pl)
    {
        this.cp = _cp;
        this.platformFlag = _platformFlag;
        this.sender = _sender;
        this.playerOwner = _pl;
    }
}
