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
    private float initialTransformX;
    private Transform initialTransform;
    private float stoppageTime = 0.0f;
    private ContactPoint lastCp;

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
        this.gameObject.rigidbody.freezeRotation = true;
        this.BallGameObject = GameObject.Find("Ball");
        this.RigBodyRef = this.GetComponent<Rigidbody>();
        this.BallRigRef = this.BallGameObject.GetComponent<Rigidbody>();
        this.gameObject.renderer.material.color = Color.red;
        this.BallRigRef.renderer.material.color = Color.green;
        this.initialTransformX = this.gameObject.transform.position.x;
        this.initialTransform = this.gameObject.transform;
        this.BallRigRef.freezeRotation = true;
        EventSystem.OnEndLevel += this.StopTheBall;
        EventSystem.OnEndLevel += this.BallToInitialPosition;
	}
	
	// Update is called once per frame
	void Update () {
        this.StoppageBugFix();
        this.PlatformAwayBugFix();
        //this.BallBehindThePlatformStopBugFix();
        //this.BallOutOfBordersBugFix();
        //this.TooHighVelocityBugFix();
        float horAxis = Input.GetAxis("Horizontal");

        if (!GameObject.Find("MainHelper").GetComponent<MainHelper>().GetCurrentGame().GamePaused)
        {
            if (!this.SideWallsCollisionPreDetection(horAxis))
            {
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

                        if (Input.GetButtonUp("Fire1"))
                        {
                            this.LaunchBall(1);
                            this.launched = true;
                            this.lastDirection = 1;
                        }
                    }
                }
            }
        }
	}

    protected void BallToInitialPosition(object sender, ChangeLevelEventArgs e)
    {
        this.StopTheBall();
        this.Launched = false;
        this.BallGameObject.transform.position = new Vector3(this.initialTransform.position.x, this.initialTransform.position.y, this.initialTransform.position.z + 0.2f);
    }

    protected void StopTheBall()
    {
        this.BallRigRef.velocity = Vector3.zero;
        this.BallRigRef.angularVelocity = Vector3.zero;
    }

    protected void StopTheBall(object sender, ChangeLevelEventArgs e)
    {
        this.BallRigRef.velocity = Vector3.zero;
        this.BallRigRef.angularVelocity = Vector3.zero;
    }

    protected bool SideWallsCollisionPreDetection(float axisValue) // returns true if collision object is at 0.1f distance false if not, whic basically means that methond returns true if collision would have taken place
    {
        Vector3 checkDirection = Vector3.zero;
        if (this.gameObject.transform.position.x > this.initialTransformX)
        {
            checkDirection = Vector3.right;
            if (axisValue < 0)
            {
                return false;
            }
        }
        else if (this.gameObject.transform.position.x < this.initialTransformX)
        {
            checkDirection = Vector3.left;
            if (axisValue > 0)
            {
                return false;
            }
        }
        else
        {
            return false;
        }

        RaycastHit hit;
        float distanceToCollision = 0.6f;
        if (Physics.Raycast(this.transform.position, checkDirection, out hit, distanceToCollision))
        {
            if (hit.collider.gameObject.tag == "wall")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public void LaunchBall(int direction)
    {
        this.BallRigRef.AddForce(transform.forward * this.thrust * direction);
        this.lastDirection = direction;
    }

    public void LaunchBall(ContactPoint cp, GameObject collidedWith, GameObject rotationDummy, int multiplier)
    {
        Debug.Log("IN");
        Quaternion initialRotation = rotationDummy.transform.rotation;
        this.StopTheBall();
        this.BallRigRef.freezeRotation = false;
        Vector3 newDir = Vector3.zero;
        Vector3 curDir = this.BallRigRef.transform.TransformDirection(Vector3.forward);
        float differenceBetweenContactAndCenter = ((cp.point.x - collidedWith.transform.position.x) + 0.1f) * multiplier;
        Debug.Log(differenceBetweenContactAndCenter.ToString());
        float modifier = 60.0f * differenceBetweenContactAndCenter;
        rotationDummy.transform.Rotate(0.0f, modifier, 0.0f);
        Quaternion targetRotation = rotationDummy.transform.rotation;
        this.BallRigRef.transform.rotation = targetRotation;
        Vector3 targVector = this.BallRigRef.transform.TransformDirection(Vector3.forward);
        this.BallRigRef.AddForce(targVector * this.thrust);
        this.BallRigRef.freezeRotation = true;
        rotationDummy.transform.rotation = initialRotation;
    }

    public void LaunchBall(ContactPoint cp, bool platformFlag)
    {
        this.StopTheBall();
        this.BallRigRef.freezeRotation = false;
        Vector3 newDir = Vector3.zero;
        Vector3 curDir = this.BallRigRef.transform.TransformDirection(Vector3.forward);
        
        float differenceBetweenContactAndCenter = cp.point.x - this.gameObject.transform.position.x;

        if (platformFlag) //hit the platform
        {
            /*if (Mathf.Abs(differenceBetweenContactAndCenter) < 0.1f) // presuming it hit the center
            {
                newDir = Vector3.Reflect(curDir, cp.normal);
                LaunchBallStraight(newDir, curDir);
                this.BallRigRef.freezeRotation = true;
            }
            else*/ // hit the side
            {
                if (differenceBetweenContactAndCenter == 0) differenceBetweenContactAndCenter = 0.1f;
                float modifier = 60.0f * differenceBetweenContactAndCenter;
                GameObject.Find("RotationDummy").transform.Rotate(0.0f, modifier, 0.0f);
                Quaternion targetRotation = GameObject.Find("RotationDummy").transform.rotation;
                this.BallRigRef.transform.rotation = targetRotation;
                Vector3 targVector = this.BallRigRef.transform.TransformDirection(Vector3.forward);
                this.BallRigRef.AddForce(targVector * this.thrust);
                this.BallRigRef.freezeRotation = true;
                GameObject.Find("RotationDummy").transform.rotation = Quaternion.identity;
            }
        }
        else // hit something else
        {
            newDir = Vector3.Reflect(curDir, cp.normal);
            if (this.CheckForStraightAngleBug(newDir))
            {
                Vector3 tmpDir = new Vector3(newDir.x + (Random.Range(0.5f, 1.5f) - 1.0f), 0.0f, (Random.Range(0.5f, 1.5f) - 1.0f));
                newDir = tmpDir;
            }
            LaunchBallStraight(newDir, curDir);
            this.BallRigRef.freezeRotation = true;
        }

        this.lastCp = cp;
    }

    public void LaunchBall(ContactPoint cp, bool platformFlag, Vector3 reflectionVector)
    {
        this.StopTheBall();
        this.BallRigRef.freezeRotation = false;
        Vector3 newDir = Vector3.zero;
        Vector3 curDir = this.BallRigRef.transform.TransformDirection(Vector3.forward);

        float differenceBetweenContactAndCenter = cp.point.x - this.gameObject.transform.position.x;

        if (platformFlag) //hit the platform
        {
            if (Mathf.Abs(differenceBetweenContactAndCenter) < 0.1f) // presuming it hit the center
            {
                newDir = Vector3.Reflect(curDir, cp.normal);
                LaunchBallStraight(newDir, curDir);
                this.BallRigRef.freezeRotation = true;
            }
            else // hit the side
            {
                float modifier = 60.0f * differenceBetweenContactAndCenter;
                GameObject.Find("RotationDummy").transform.Rotate(0.0f, modifier, 0.0f);
                Quaternion targetRotation = GameObject.Find("RotationDummy").transform.rotation;
                this.BallRigRef.transform.rotation = targetRotation;
                Vector3 targVector = this.BallRigRef.transform.TransformDirection(Vector3.forward);
                this.BallRigRef.AddForce(targVector * this.thrust);
                this.BallRigRef.freezeRotation = true;
                GameObject.Find("RotationDummy").transform.rotation = Quaternion.identity;
            }
        }
        else // hit something else
        {
            newDir = Vector3.Reflect(curDir, reflectionVector);
            if (this.CheckForStraightAngleBug(newDir))
            {
                Vector3 tmpDir = new Vector3(newDir.x + (Random.Range(0.5f, 1.5f) - 1.0f), 0.0f, (Random.Range(0.5f, 1.5f) - 1.0f));
                newDir = tmpDir;
            }
            LaunchBallStraight(newDir, curDir);
            this.BallRigRef.freezeRotation = true;
        }

        this.lastCp = cp;
    }

    protected void LaunchBallStraight(Vector3 newDir, Vector3 curDir)
    {
        this.BallRigRef.rotation = Quaternion.FromToRotation(Vector3.forward, newDir);
        
        this.BallRigRef.AddForce(newDir * this.thrust);
    }

    public void SimpleReflect(ContactPoint cp) //currently unused, remove later
    {
        this.BallRigRef.velocity = Vector3.zero;
        this.BallRigRef.angularVelocity = Vector3.zero;
        Vector3 newDir = Vector3.zero;
        Vector3 curDir = this.BallRigRef.transform.TransformDirection(Vector3.forward);
        
        newDir = Vector3.Reflect(curDir, cp.normal);

        this.BallRigRef.rotation = Quaternion.FromToRotation(Vector3.forward, newDir);;
        this.BallRigRef.AddForce(newDir * this.thrust);
    }

    void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.name == this.BallGameObject.name && this.launched)
        {
            this.LaunchBall(c.contacts[0], true);
        }
    }

    #region BugFixes

    protected bool CheckForStraightAngleBug(Vector3 newDir)
    {
        float vectorX = Mathf.Abs(newDir.x) - 1.0f;
        float vectorZ = Mathf.Abs(newDir.z) - 1.0f;

        if (vectorX > -0.00001f && Mathf.Abs(newDir.z) < 0.00001f)
            return true;
        else
            return false;
    }

    protected void BallBehindThePlatformStopBugFix()
    {
        if (this.BallGameObject.transform.position.z < this.gameObject.transform.position.z)
        {
            this.stoppageTime += Time.deltaTime;

            if (this.stoppageTime > 0.5f)
            {
                this.stoppageTime = 0.0f;
                GameObject.Find("BackWall").GetComponent<BackWallHandler>().BallCrush("Ball");
            }
        }
    }

    protected void PlatformAwayBugFix()
    {
        float deltaZ = Mathf.Abs(this.initialTransform.position.z - this.gameObject.transform.position.z);
        float deltaY = Mathf.Abs(this.initialTransform.position.y - this.gameObject.transform.position.y);

        if (deltaY > 0.1f || deltaZ > 0.1f)
        {
            this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x, this.initialTransform.position.y, this.initialTransform.position.z);
        }
    }

    protected void BallOutOfBordersBugFix()
    {
        Vector3 inititalVelocityVector = this.BallRigRef.velocity;
        if (this.BallGameObject.transform.position.z > GameObject.Find("FrontWall").transform.position.z)
        {
            this.StopTheBall();
            GameObject go = GameObject.Find("FrontWall");
            this.BallGameObject.transform.position = new Vector3(go.transform.position.x, this.BallGameObject.transform.position.y, go.transform.position.z - 0.2f);
            Vector3 newDir = Vector3.Reflect(inititalVelocityVector, Vector3.forward);
            this.LaunchBallStraight(newDir, inititalVelocityVector);
        }
        else if (this.BallGameObject.transform.position.x < GameObject.Find("LeftWall").transform.position.x)
        {
            this.StopTheBall();
            GameObject go = GameObject.Find("LeftWall");
            this.BallGameObject.transform.position = new Vector3(go.transform.position.x + 0.2f, this.BallGameObject.transform.position.y, go.transform.position.z);
            Vector3 newDir = Vector3.Reflect(inititalVelocityVector, Vector3.right);
            this.LaunchBallStraight(newDir, inititalVelocityVector);
        }
        else if (this.BallGameObject.transform.position.x > GameObject.Find("RightWall").transform.position.x)
        {
            this.StopTheBall();
            GameObject go = GameObject.Find("RightWall");
            this.BallGameObject.transform.position = new Vector3(go.transform.position.x - 0.2f, this.BallGameObject.transform.position.y, go.transform.position.z);
            Vector3 newDir = Vector3.Reflect(inititalVelocityVector, Vector3.left);
            this.LaunchBallStraight(newDir, inititalVelocityVector);
        }
    }

    protected void StoppageBugFix()
    {
        if (this.launched)
        {
            if (this.BallRigRef.velocity.Equals(Vector3.zero))
            {
                this.stoppageTime += Time.deltaTime;
            }
            else
            {
                if (this.stoppageTime > 0.0f)
                    this.stoppageTime = 0.0f;
            }
        }

        if (this.stoppageTime >= 0.15f)
        {
            this.LaunchBall(this.lastCp, false);
        }
    }

    protected void TooHighVelocityBugFix()
    {
        if (this.BallRigRef.velocity.magnitude > 4.5f)
        {
            Vector3 initialVelocity = this.BallRigRef.velocity;
            this.StopTheBall();
            //this.BallRigRef.AddForce((initialVelocity / 2) * this.thrust);
        }
    }

    #endregion
}
