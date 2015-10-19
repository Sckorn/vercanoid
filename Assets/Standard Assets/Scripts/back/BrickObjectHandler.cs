using UnityEngine;
using System.Collections;

public class BrickObjectHandler : MonoBehaviour {
    private int hitsToKill;
    private int hitsTookPlace = 0;
    private GridCellCoords coordinates;
    private Color[] choosedColors = { Color.blue, Color.cyan, Color.gray, Color.green,Color.red, Color.yellow, Color.magenta, new Color(0.188f, 0.316f, 0.316f, 1.0f), new Color(1.0f, 0.86f, 0.0f, 1.0f)};
    public int pointsPerHit = 10;
    private MainHelper mhReference;
    private Object thisLock = new Object();

    public int HitsToKill
    {
        get { return this.hitsToKill; }
        set { this.hitsToKill = value; }
    }

    public void SetCoordinates(GridCellCoords c)
    {
        this.coordinates.x = c.x;
        this.coordinates.y = c.y;
    }

	// Use this for initialization
	void Start () {
        gameObject.renderer.material.color = this.choosedColors[Random.Range(0, this.choosedColors.Length)];
        this.mhReference = GameObject.Find("MainHelper").GetComponent<MainHelper>();
	}
	
	// Update is called once per frame
	void Update () {
        if (this.hitsTookPlace == this.hitsToKill)
        {
            Destroy(gameObject);
            this.mhReference.GetCurrentGame().GetCurrentField().BrickDestroyed(this.coordinates);
            this.mhReference.GetCurrentGame().GetHumanPlayer().IncreaseLevelScore(this.hitsToKill * this.pointsPerHit);
            InterfaceUpdateEventArgs e = new InterfaceUpdateEventArgs(InterfaceUpdateReasons.ScoreIncreased, this.mhReference.GetCurrentGame().GetHumanPlayer().GetLevelScore().ToString());
            EventSystem.FireInterfaceUpdate(this.mhReference.GetCurrentGame().GetHumanPlayer(), e);
        }
	}

    protected void SelfDestruct(object sender, ChangeLevelEventArgs e)
    {
        if(this.gameObject != null)
            Destroy(this.gameObject);
    }

    void OnCollisionEnter(Collision c)
    {
            if (c.gameObject.name == "Ball")
            {
                //GameObject.Find("Platform").GetComponent<PlatformMover>().LaunchBall(c.contacts[0], false);
                DelayedCollision dc = new DelayedCollision(c.contacts[0], false);
                GameObject.Find("MainHelper").GetComponent<MainHelper>().AddCollisionToQueue(dc);
                /*if(c.contacts[0].point.z > this.gameObject.transform.position.z)
                    GameObject.Find("Platform").GetComponent<PlatformMover>().LaunchBall(c.contacts[0], this.gameObject, GameObject.Find("RotationDummy"), 1);
                else
                    GameObject.Find("Platform").GetComponent<PlatformMover>().LaunchBall(c.contacts[0], this.gameObject, GameObject.Find("ReverseRotationDummy"), -1);*/
                ++this.hitsTookPlace;
            }
            /*if (this.hitsTookPlace == this.hitsToKill)
            {
                if (c.gameObject.name == "Ball")
                {
                    //GameObject.Find("Platform").GetComponent<PlatformMover>().LaunchBall(c.contacts[0], false);
                    GameObject.Find("Platform").GetComponent<PlatformMover>().LaunchBall(c.contacts[0], this.gameObject);
                }
            }
            else
            {*/
                
            //}
    }
}
