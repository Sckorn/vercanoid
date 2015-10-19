using UnityEngine;
using System.Collections;

public class BackWallHandler : MonoBehaviour {
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.name == "Ball")
        {
            this.BallCrush(c.gameObject.name);
        }
    }

    public void BallCrush(string collisionObjectName)
    {
        Transform plateTransform = GameObject.Find("Platform").transform;
        GameObject ballObj = GameObject.Find(collisionObjectName);
        ballObj.GetComponent<Rigidbody>().velocity = Vector3.zero;
        ballObj.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        Vector3 newBallPosition = new Vector3(plateTransform.position.x, 0.25f, plateTransform.position.z + 0.2f);

        ballObj.transform.position = newBallPosition;
        ballObj.transform.rotation = Quaternion.identity;
        PlatformMover pm = GameObject.Find("Platform").GetComponent<PlatformMover>();
        pm.Launched = false;
        object sender = new object();
        BallCrushedEventArgs e = new BallCrushedEventArgs(BallCrushReasons.PlayerBackWallCrush, 0);
        EventSystem.FireBallCrush(sender, e);
    }
}
