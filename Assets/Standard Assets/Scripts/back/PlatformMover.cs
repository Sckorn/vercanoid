using UnityEngine;
using System.Collections;

public class PlatformMover : MonoBehaviour {
    public float speed = 5.0f;
    private GameObject BallGameObject;
    private Rigidbody RigBodyRef;
    private Rigidbody BallRigRef;
    public float thrust = 10.0f;
    private bool launched = false;
    private int lastDirection = 0;
    private float lastCollisionX = 10.0f;

    public int LastDirection
    {
        get { return this.lastDirection; }
        set { this.lastDirection = value; }
    }

    public bool Launched
    {
        get { return this.launched; }
        set { this.launched = value; }
    }

	// Use this for initialization
	void Start () {
        this.BallGameObject = GameObject.Find("Ball");
        this.RigBodyRef = this.GetComponent<Rigidbody>();
        this.BallRigRef = this.BallGameObject.GetComponent<Rigidbody>();
        this.gameObject.renderer.material.color = Color.red;
        this.BallRigRef.renderer.material.color = Color.green;
	}
	
	// Update is called once per frame
	void Update () {
        float horAxis = Input.GetAxis("Horizontal");

        if (GameObject.Find("MainHelper").GetComponent<MainHelper>().GetCurrentGame().GameInProgress)
        {
            this.RigBodyRef.MovePosition(new Vector3(
                this.transform.position.x + (horAxis * Time.deltaTime * this.speed),
                this.transform.position.y,
                this.transform.position.z
                )
            );

            if (!this.launched)
            {
                this.BallRigRef.MovePosition(new Vector3(
                    this.BallGameObject.transform.position.x + (horAxis * Time.deltaTime * this.speed),
                    this.BallGameObject.transform.position.y,
                    this.BallGameObject.transform.position.z
                    )
                );

                if (Input.GetKeyUp(KeyCode.Space))
                {
                    this.LaunchBall(1);
                    this.launched = true;
                    this.lastDirection = 1;
                }
            }
        }
	}

    public void LaunchBall(int direction)
    {
        this.BallRigRef.AddForce(transform.forward * this.thrust * direction);
        this.lastDirection = direction;
    }

    public void LaunchBall(ContactPoint cp, bool platformFlag)
    {
        this.BallRigRef.velocity = Vector3.zero;
        this.BallRigRef.angularVelocity = Vector3.zero;
        Vector3 newDir = Vector3.zero;
        Vector3 curDir = this.BallRigRef.transform.TransformDirection(Vector3.forward);
        
        float differenceBetweenContactAndCenter = cp.point.x - this.gameObject.transform.position.x;

        if (platformFlag)
        {
            if (Mathf.Abs(differenceBetweenContactAndCenter) < 0.1) // presuming it hit the center
            {
                newDir = Vector3.Reflect(curDir, cp.normal);
                LaunchBallStraight(newDir, curDir);
            }
            else // hit the side
            {
                float modifier = 60.0f * differenceBetweenContactAndCenter;
                GameObject.Find("RotationDummy").transform.Rotate(0.0f, modifier, 0.0f);
                Quaternion targetRotation = GameObject.Find("RotationDummy").transform.rotation;
                this.BallRigRef.transform.rotation = targetRotation;
                Vector3 targVector = this.BallRigRef.transform.TransformDirection(Vector3.forward);
                this.BallRigRef.AddForce(targVector * this.thrust);

                GameObject.Find("RotationDummy").transform.rotation = Quaternion.identity;
            }
        }
        else
        {
            newDir = Vector3.Reflect(curDir, cp.normal);
            LaunchBallStraight(newDir, curDir);
        }
    }

    protected void LaunchBallStraight(Vector3 newDir, Vector3 curDir)
    {
        this.BallRigRef.rotation = Quaternion.FromToRotation(Vector3.forward, newDir);

        this.BallRigRef.AddForce(newDir * this.thrust);
    }

    public void SimpleReflect(ContactPoint cp)
    {
        this.BallRigRef.velocity = Vector3.zero;
        this.BallRigRef.angularVelocity = Vector3.zero;
        Vector3 newDir = Vector3.zero;
        Vector3 curDir = this.BallRigRef.transform.TransformDirection(Vector3.forward);
        
        newDir = Vector3.Reflect(curDir, cp.normal);

        this.BallRigRef.rotation = Quaternion.FromToRotation(Vector3.forward, newDir);
        this.BallRigRef.AddForce(newDir * this.thrust);
    }

    void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.name == this.BallGameObject.name && this.launched)
        {
            this.LaunchBall(c.contacts[0], true);
        }
    }
}
