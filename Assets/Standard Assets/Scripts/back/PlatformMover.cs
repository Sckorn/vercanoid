using UnityEngine;
using System.Collections;

public class PlatformMover : MonoBehaviour {
    public float speed = 5.0f;
    private GameObject BallGameObject;
    public GameObject ConnectedBallObject = null;
    private Rigidbody RigBodyRef;
    private Rigidbody BallRigRef;
    public float thrust = 10.0f;
    private bool launched = false;
    private int lastDirection = 0;
    private float initialTransformX;
    private Transform initialTransform;
    private Transform ballInitialTransform;
    private float stoppageTime = 0.0f;
    private ContactPoint lastCp;
    private Players belongsToPlayer;

    public bool bShowBallVelocity = false;

    public Players BelongsToPlayer
    {
        get { return this.belongsToPlayer; }
        set { this.belongsToPlayer = value; }
    }

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
        Debug.LogError("Current game mode " + Globals.CurrentGameMode.ToString());
        if (gameObject.tag.Equals("SecondPlayerPlatform"))
        {
            this.belongsToPlayer = Players.SecondPlayer;
            this.ConnectedBallObject.GetComponent<BallCollisionHandler>().BelongsToPlayer = Players.SecondPlayer;
        }
        else if (gameObject.tag.Equals("FirstPlayerPlatform"))
        {
            this.belongsToPlayer = Players.FirstPlayer;
            this.ConnectedBallObject.GetComponent<BallCollisionHandler>().BelongsToPlayer = Players.FirstPlayer;
        }
        else
        {
            this.belongsToPlayer = Players.FirstPlayer;
            this.ConnectedBallObject.GetComponent<BallCollisionHandler>().BelongsToPlayer = Players.FirstPlayer;
        }

        this.gameObject.rigidbody.freezeRotation = true;

        if (Globals.CurrentGameMode == GameModes.Versus)
        {
            this.BallGameObject = this.ConnectedBallObject;
        }
        else
        {
            this.BallGameObject = GameObject.Find("Ball");
        }
        this.RigBodyRef = this.GetComponent<Rigidbody>();
        this.BallRigRef = this.BallGameObject.GetComponent<Rigidbody>();

        this.initialTransformX = gameObject.transform.position.x;
        this.initialTransform = this.gameObject.transform;
        this.ballInitialTransform = this.BallGameObject.transform;
        this.BallRigRef.freezeRotation = true;
        EventSystem.OnEndLevel += this.StopTheBall;
        EventSystem.OnEndLevel += this.BallToInitialPosition;
        EventSystem.OnEndGame += this.BallToInitialPosition;
        EventSystem.OnEndGame += this.StopTheBall;

        if (Globals.CurrentGameMode == GameModes.Versus)
        {
            if (this.gameObject.tag.Equals("FirstPlayerPlatform"))
            {
                this.gameObject.renderer.material.color = Color.white;
                this.BallGameObject.renderer.material.color = Color.red;
            }

            if (this.gameObject.tag.Equals("SecondPlayerPlatform"))
            {
                this.gameObject.renderer.material.color = Color.gray;
                this.BallGameObject.renderer.material.color = Color.blue;
            }            
        }
        //Time.timeScale = 0;
	}
	
	// Update is called once per frame
	void Update () {
        if(this.bShowBallVelocity)
            Debug.LogWarning("Ball belongs to " + this.BelongsToPlayer.ToString() + " Ball's current velocity " + this.BallRigRef.velocity.magnitude.ToString());
        //this.BallGameObject = GameObject.Find("Ball");
        this.StoppageBugFix();
        this.PlatformAwayBugFix();
        //this.BallBehindThePlatformStopBugFix();
        //this.BallOutOfBordersBugFix();
        //this.TooHighVelocityBugFix();
        float horAxis = Input.GetAxis("Horizontal");
        string buttonToFire = "Fire1";
        float boundaryLeft = 1.3f;
        float boundaryRight = 18.61f;
        if (Globals.CurrentGameMode == GameModes.Versus)
        {
            if (this.gameObject.tag.Equals("SecondPlayerPlatform"))
            {
                horAxis = Input.GetAxis("HorizontalSecond");
                buttonToFire = "Fire2";
            }
        }

        if (Input.GetKeyUp(KeyCode.Q)) this.LaunchBall(1);

        if (!GameObject.Find("MainHelper").GetComponent<MainHelper>().GetCurrentGame().GamePaused)
        {
            if (!this.SideWallsCollisionPreDetection(horAxis))
            {
                if (GameObject.Find("MainHelper").GetComponent<MainHelper>().GetCurrentGame().GameInProgress)
                {
                    if (this.gameObject.transform.position.x - horAxis > boundaryLeft || this.gameObject.transform.position.x + horAxis < boundaryRight) //additional check in case physics engine cannot calculate raycast in time to prevent collision
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

                            if (Input.GetButtonUp(buttonToFire))
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
	}

    protected void BallToInitialPosition(object sender, ChangeLevelEventArgs e)
    {
        this.BallToInitialPosition();
    }

    protected void BallToInitialPosition(object sender, EndGameEventArgs e)
    {
        this.BallToInitialPosition();
    }

    public void BallToInitialPosition()
    {
        this.StopTheBall();
        this.Launched = false;
        float addition = 0.0f;
        if (this.belongsToPlayer == Players.FirstPlayer) addition = +0.2f; else addition = -0.2f;
        this.BallGameObject.transform.position = /*this.ballInitialTransform.position;*/new Vector3(this.initialTransform.position.x, this.initialTransform.position.y, this.initialTransform.position.z + addition);
        if (this.belongsToPlayer == Players.FirstPlayer)
            this.BallGameObject.transform.rotation = this.ballInitialTransform.rotation;
        else
        {
            this.BallGameObject.transform.rotation = Quaternion.identity;//this.ballInitialTransform.rotation;
            this.BallGameObject.GetComponent<Rigidbody>().freezeRotation = false;
            this.BallGameObject.transform.Rotate(0.0f, 180.0f, 0.0f);
            this.BallGameObject.GetComponent<Rigidbody>().freezeRotation = true;
        }
    }

    protected void StopTheBall()
    {
        this.BallRigRef.velocity = Vector3.zero;
        this.BallRigRef.angularVelocity = Vector3.zero;
    }

    protected void StopTheBall(object sender, ChangeLevelEventArgs e)
    {
        this.StopTheBall();
    }

    protected void StopTheBall(object sender, EndGameEventArgs e)
    {
        this.StopTheBall();
    }

    protected bool SideWallsCollisionPreDetection(float axisValue) // returns true if collision object is at 0.1f distance false if not, which basically means that methond returns true if collision would have taken place
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
        this.BallRigRef.AddForce(this.gameObject.transform.forward * this.thrust * direction);
        this.lastDirection = direction;
    }

    #region valid_launch_ball_for_single
    public void LaunchBall(ContactPoint cp, bool platformFlag)
    {
        this.StopTheBall();
        this.BallRigRef.freezeRotation = false;
        /*Vector3 newDir = Vector3.zero;
        Vector3 curDir = this.BallRigRef.transform.TransformDirection(Vector3.forward);
        
        float differenceBetweenContactAndCenter = cp.point.x - this.gameObject.transform.position.x;

        if (platformFlag) //hit the platform
        {
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

        this.lastCp = cp;*/
        Vector3 newDir = Vector3.Reflect(this.BallGameObject.transform.forward, cp.normal);
        if (this.CheckForStraightAngleBug(newDir))
        {
            Debug.LogError("Straight angle");
            Vector3 tmpDir = new Vector3(newDir.x + (Random.Range(0.5f, 1.5f) - 1.0f), 0.0f, (Random.Range(0.5f, 1.5f) - 1.0f));
            newDir = tmpDir;
        }
        Quaternion newRot = Quaternion.FromToRotation(this.BallGameObject.transform.forward, newDir);
        this.BallGameObject.transform.rotation = Quaternion.LookRotation(newDir);
        this.BallGameObject.transform.localEulerAngles = new Vector3(0.0f, this.BallGameObject.transform.rotation.eulerAngles.y, this.BallGameObject.transform.rotation.eulerAngles.z);
        if (platformFlag)
        {
            float differenceBetweenContactAndCenter = cp.point.x - this.gameObject.transform.position.x;
            if (differenceBetweenContactAndCenter == 0) differenceBetweenContactAndCenter = 0.1f;
            float modifier = 60.0f * differenceBetweenContactAndCenter;
            this.BallGameObject.transform.Rotate(0.0f, modifier, 0.0f);
        }

        this.BallRigRef.AddForce(this.BallGameObject.transform.forward * this.thrust);
        this.BallRigRef.freezeRotation = true;
    }
    #endregion

    public void LaunchBall(ContactPoint cp, bool platformFlag, Players instigator)
    {
        this.StopTheBall();
        this.BallRigRef.freezeRotation = false;
        /*Vector3 newDir = Vector3.zero;

        Vector3 relationVector;
        string dummyName = string.Empty;

        if (instigator == Players.FirstPlayer)
        {
            relationVector = this.gameObject.transform.forward;//Vector3.forward;
            dummyName = "RotationDummy";
        }
        else
        {
            relationVector = this.gameObject.transform.forward;//Vector3.forward;
            dummyName = "RotationDummy";
        }*/

        //Vector3 curDir = /*relationVector;*/this.BallRigRef.transform.TransformDirection(relationVector);
        //Debug.LogError("Current direction local " + this.gameObject.transform.forward + " Current World direction "  + curDir.ToString() + " World forward direction " + Vector3.forward.ToString());

        /*float differenceBetweenContactAndCenter = cp.point.x - this.gameObject.transform.position.x;

        if (platformFlag) //hit the platform
        {
            //Debug.LogError("Collided with mah platform");
            //Time.timeScale = 0;
            if (differenceBetweenContactAndCenter == 0) differenceBetweenContactAndCenter = 0.1f;
            float modifier = 60.0f * differenceBetweenContactAndCenter;
            //Debug.LogError(modifier.ToString());
            if (instigator == Players.SecondPlayer) modifier *= -1;
            GameObject.Find(dummyName).transform.Rotate(0.0f, modifier, 0.0f);
            Quaternion targetRotation = GameObject.Find(dummyName).transform.rotation;
            //Debug.LogError("Target rotation " + targetRotation.eulerAngles.ToString());
            this.BallRigRef.transform.rotation = targetRotation;
            Vector3 targVector = this.BallRigRef.transform.TransformDirection(relationVector);
            this.BallRigRef.AddForce(targVector * this.thrust);
            //Debug.LogError("Target direction " + targVector.ToString());
            this.BallRigRef.freezeRotation = true;
            GameObject.Find(dummyName).transform.rotation = Quaternion.identity;
        }
        else // hit something else
        {
            Debug.LogError("Contact point normal " + cp.normal.ToString());
            //if (this.belongsToPlayer == Players.SecondPlayer) curDir = relationVector;
            if (curDir == Vector3.forward || curDir == Vector3.back) curDir = relationVector; // figure out why it doesn't fucking work with straight direction
            //curDir = relationVector;
            Debug.LogError("Collided with mah brick");
            newDir = Vector3.Reflect(curDir, cp.normal);
            Debug.LogError("Current direction " + curDir.ToString() + " New Direction " + newDir.ToString() + " Global curDir " + this.BallRigRef.transform.TransformDirection(relationVector).ToString() + " Global newDir " + Vector3.Reflect(this.BallRigRef.transform.TransformDirection(relationVector), cp.normal).ToString());
            if (this.CheckForStraightAngleBug(newDir))
            {
                Debug.LogError("Straight angle");
                Vector3 tmpDir = new Vector3(newDir.x + (Random.Range(0.5f, 1.5f) - 1.0f), 0.0f, (Random.Range(0.5f, 1.5f) - 1.0f));
                newDir = tmpDir;
            }
            LaunchBallStraight(newDir, curDir);
            this.BallRigRef.freezeRotation = true;
        }

        this.lastCp = cp;*/
        Vector3 newDir = Vector3.Reflect(this.BallGameObject.transform.forward, cp.normal);
        if (this.CheckForStraightAngleBug(newDir))
        {
            Debug.LogError("Straight angle");
            Vector3 tmpDir = new Vector3(newDir.x + (Random.Range(0.5f, 1.5f) - 1.0f), 0.0f, (Random.Range(0.5f, 1.5f) - 1.0f));
            newDir = tmpDir;
        }
        Quaternion newRot = Quaternion.FromToRotation(this.BallGameObject.transform.forward, newDir);
        this.BallGameObject.transform.rotation = Quaternion.LookRotation(newDir);
        this.BallGameObject.transform.localEulerAngles = new Vector3(0.0f, this.BallGameObject.transform.rotation.eulerAngles.y, this.BallGameObject.transform.rotation.eulerAngles.z);
        if (platformFlag)
        {
            float differenceBetweenContactAndCenter = cp.point.x - this.gameObject.transform.position.x;
            if (differenceBetweenContactAndCenter == 0) differenceBetweenContactAndCenter = 0.1f;
            float modifier = 60.0f * differenceBetweenContactAndCenter;
            if (instigator == Players.SecondPlayer) modifier *= -1;
            this.BallGameObject.transform.Rotate(0.0f, modifier, 0.0f);
        }

        this.BallRigRef.AddForce(this.BallGameObject.transform.forward * this.thrust);
        this.BallRigRef.freezeRotation = true;
    }

    public void LaunchBall(ContactPoint cp, bool platformFlag, Vector3 reflectionVector, Players invokingPlayer = Players.FirstPlayer)
    {
        this.StopTheBall();
        this.BallRigRef.freezeRotation = false;
        Vector3 newDir = Vector3.zero;
        Vector3 curDir = this.BallRigRef.transform.TransformDirection(Vector3.forward);

        if (invokingPlayer == Players.SecondPlayer)
        {
            curDir = this.BallRigRef.transform.TransformDirection(this.gameObject.transform.forward);
        }

        float differenceBetweenContactAndCenter = cp.point.x - this.gameObject.transform.position.x;

        if (platformFlag) //hit the platform
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
        if(Globals.CurrentGameMode == GameModes.SinglePlayer)
            this.BallRigRef.rotation = Quaternion.FromToRotation(Vector3.forward, newDir);
        else
            this.BallRigRef.rotation = Quaternion.FromToRotation(curDir, newDir);
        this.BallRigRef.AddForce(newDir * this.thrust);
    }

    void OnCollisionEnter(Collision c)
    {
        if (Globals.CurrentGameMode == GameModes.SinglePlayer)
        {
            if (c.gameObject.name == this.BallGameObject.name && this.launched)
            {
                this.LaunchBall(c.contacts[0], true);
            }
        }
        else
        {
            if (c.gameObject.tag.Equals("PlayerBall") && this.launched)
            {
                c.gameObject.GetComponent<BallCollisionHandler>().MotherPlatform.GetComponent<PlatformMover>().LaunchBall(c.contacts[0], true, c.gameObject.GetComponent<BallCollisionHandler>().BelongsToPlayer);
            }
        }
    }

    void OnDestroy()
    {
        EventSystem.OnEndLevel -= this.StopTheBall;
        EventSystem.OnEndLevel -= this.BallToInitialPosition;
        EventSystem.OnEndGame -= this.BallToInitialPosition;
        EventSystem.OnEndGame -= this.StopTheBall;
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
        }
    }

    #endregion
}
