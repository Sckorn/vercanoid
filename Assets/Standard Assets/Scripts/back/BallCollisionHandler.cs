using UnityEngine;
using System.Collections;

public class BallCollisionHandler : MonoBehaviour {

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
