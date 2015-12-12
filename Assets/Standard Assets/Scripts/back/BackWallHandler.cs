using UnityEngine;
using System;
using System.Collections;

public class BackWallHandler : MonoBehaviour {
    public GameObject CorrespondingPlatform;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter(Collision c)
    {
        if (Globals.CurrentGameMode == GameModes.SinglePlayer)
        {
            if (c.gameObject.name == "Ball")
            {
                this.BallCrush(c.gameObject.name);
            }
        }
        else
        {
            if (c.gameObject.tag == "PlayerBall")
            {
                GameObject platform = null;
                try
                {
                    platform = c.gameObject.GetComponent<BallCollisionHandler>().MotherPlatform;
                }
                catch (Exception e)
                {
                    Debug.Log("Null mother platform object");
                    Debug.Log(e.Message);
                    //return;
                }

                if (platform == null)
                {
                    if (c.gameObject.GetComponent<BallCollisionHandler>().BelongsToPlayer == Players.FirstPlayer)
                    {
                        platform = GameObject.Find("FirstPlayerPlatform");
                    }
                    else
                    {
                        platform = GameObject.Find("SecondPlayerPlatform");
                    }
                }

                if (!c.gameObject.Equals(this.CorrespondingPlatform.GetComponent<PlatformMover>().ConnectedBallObject))
                {
                    this.BallCrush(this.CorrespondingPlatform.GetComponent<PlatformMover>().ConnectedBallObject, this.CorrespondingPlatform);
                    c.gameObject.GetComponent<BallCollisionHandler>().MotherPlatform.GetComponent<PlatformMover>().BallToInitialPosition();
                }
                else
                {
                    this.BallCrush(c.gameObject, platform);
                }
            }
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

    public void BallCrush(GameObject collisionObject, GameObject correspondingPlatform)
    {
        Debug.LogError("Ball crushed");
        Transform plateTransform = correspondingPlatform.transform;
        GameObject ballObj = collisionObject;
        ballObj.GetComponent<Rigidbody>().velocity = Vector3.zero;
        ballObj.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        Vector3 newBallPosition = Vector3.zero;
        if(collisionObject.GetComponent<BallCollisionHandler>().BelongsToPlayer == Players.FirstPlayer)
            newBallPosition = new Vector3(plateTransform.position.x, 0.25f, plateTransform.position.z + 0.2f);

        if(collisionObject.GetComponent<BallCollisionHandler>().BelongsToPlayer == Players.SecondPlayer)
            newBallPosition = new Vector3(plateTransform.position.x, 0.25f, plateTransform.position.z - 0.2f);

        ballObj.transform.position = newBallPosition;
        ballObj.transform.rotation = Quaternion.identity;
        PlatformMover pm = correspondingPlatform.GetComponent<PlatformMover>();
        pm.Launched = false;
        object sender = new object();
        BallCrushedEventArgs e = new BallCrushedEventArgs(BallCrushReasons.PlayerBackWallCrush, 0, collisionObject.GetComponent<BallCollisionHandler>().BelongsToPlayer);
        EventSystem.FireBallCrush(sender, e);
    }
}
