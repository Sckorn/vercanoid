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
        //this.BallRigRef.velocity = Vector3.zero;
        //this.BallRigRef.angularVelocity = Vector3.zero;
        this.BallRigRef.AddForce(transform.forward * this.thrust * direction);
        this.lastDirection = direction;
    }

    public void LaunchBall(ContactPoint cp)
    {
        this.BallRigRef.velocity = Vector3.zero;
        this.BallRigRef.angularVelocity = Vector3.zero;
        Vector3 newDir = Vector3.zero;
        Vector3 curDir = this.BallRigRef.transform.TransformDirection(Vector3.forward);
        newDir = Vector3.Reflect(curDir, cp.normal);
        
        //Debug.Log(curDir);
        //Debug.Log(newDir);
        //Debug.Log(cp.point);

        if (cp.point.x < this.gameObject.transform.position.x)
        {
            Debug.Log("Left part");
            Debug.Log(curDir);
            Debug.Log(newDir);
            Debug.Log(cp.point);
        }
        else if (cp.point.x > this.gameObject.transform.position.x)
        {
            Debug.Log("Right Part");
            Debug.Log(curDir);
            Debug.Log(newDir);
            Debug.Log(cp.point);
        }
        else
        {
            this.BallRigRef.rotation = Quaternion.FromToRotation(Vector3.forward, newDir);

            this.BallRigRef.AddForce(newDir * this.thrust * -1);
        }
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
        Debug.Log(this.BallRigRef.rotation.ToString());
        this.BallRigRef.AddForce(newDir * this.thrust);
    }

    public void SimpleReflect(ContactPoint cp)
    {
        this.BallRigRef.velocity = Vector3.zero;
        this.BallRigRef.angularVelocity = Vector3.zero;
        Vector3 newDir = Vector3.zero;
        Vector3 curDir = this.BallRigRef.transform.TransformDirection(Vector3.forward);
        Debug.LogWarning(curDir.ToString());
        
        newDir = Vector3.Reflect(curDir, cp.normal);
        Debug.LogWarning(newDir.ToString());
        this.BallRigRef.rotation = Quaternion.FromToRotation(Vector3.forward, newDir);
        Debug.LogWarning(this.BallRigRef.rotation.ToString());
        this.BallRigRef.AddForce(newDir * this.thrust);
    }

    void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.name == this.BallGameObject.name && this.launched)
        {
            this.LaunchBall(c.contacts[0], true);
            /*this.LaunchBall(this.lastDirection * -1);
            foreach (ContactPoint cp in c.contacts)
            {
                Vector3 referenceForward = cp.normal * -1;
 
                // the vector perpendicular to referenceForward (90 degrees clockwise)
                // (used to determine if angle is positive or negative)
                Vector3 referenceRight= Vector3.Cross(Vector3.up, referenceForward);
                Vector3 heading = gameObject.transform.position - this.BallGameObject.transform.position;
                // the vector of interest
                Vector3 newDirection = heading;
 
                // Get the angle in degrees between 0 and 180
                float angle = Vector3.Angle(newDirection, referenceForward);
 
                // Determine if the degree value should be negative.  Here, a positive value
                // from the dot product means that our vector is on the right of the reference vector   
                // whereas a negative value means we're on the left.
                float sign = Mathf.Sign(Vector3.Dot(newDirection, referenceRight));
 
                float finalAngle = sign * angle;
                Debug.Log("Final Angle " + finalAngle.ToString());

                Debug.Log(cp.point.x.ToString() + " " + this.lastCollisionX.ToString());
                //Time.timeScale = 0.0f;
                Debug.DrawRay(cp.point, cp.normal, Color.red, 1000.0f);
                if (cp.point.x > this.lastCollisionX)
                {
                    this.BallRigRef.AddForce(new Vector3(-1.0f, 0.0f, 1.0f) * this.thrust);
                }
                else if(cp.point.x < this.lastCollisionX)
                {
                    this.BallRigRef.AddForce(new Vector3(1.0f, 0.0f, 1.0f) * this.thrust);
                }
                else if (cp.point.x == this.lastCollisionX)
                {
                    this.LaunchBall(1);
                }
            }*/
        }
    }
}
