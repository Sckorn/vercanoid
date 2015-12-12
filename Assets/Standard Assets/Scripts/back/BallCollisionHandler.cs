using UnityEngine;
using System.Collections;

public class BallCollisionHandler : MonoBehaviour {
    public GameObject MotherPlatform = null;
    private Players belongsToPlayer;

    public Players BelongsToPlayer
    {
        get { return this.belongsToPlayer; }
        set { this.belongsToPlayer = value; }
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnDestroy()
    {
        Debug.Log("Ball object is being destoyed");
    }
}
