using UnityEngine;
using System.Collections;

public class BrickObjectHandler : MonoBehaviour {
    private int hitsToKill;
    private int hitsTookPlace = 0;
    private GridCellCoords coordinates;
    private Color[] choosedColors = { Color.blue, Color.green, Color.red };//{ Color.blue, Color.cyan, Color.gray, Color.green,Color.red, Color.yellow, Color.magenta, new Color(0.188f, 0.316f, 0.316f, 1.0f), new Color(1.0f, 0.86f, 0.0f, 1.0f)};
    public int pointsPerHit = 10;
    private MainHelper mhReference;
    private Object thisLock = new Object();
    private Players lastHitByPlayer;

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
        if (GameObject.Find("MainHelper") != null)
        {
            gameObject.GetComponent<Renderer>().material.color = this.choosedColors[hitsToKill - 1];
            this.mhReference = GameObject.Find("MainHelper").GetComponent<MainHelper>();
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (this.hitsTookPlace == this.hitsToKill)
        {
            Destroy(gameObject);
            if (this.mhReference != null)
            {
                this.mhReference.GetCurrentGame().GetCurrentField().BrickDestroyed(this.coordinates);
                if (Globals.CurrentGameMode == GameModes.SinglePlayer)
                {
                    this.mhReference.GetCurrentGame().GetHumanPlayer().IncreaseLevelScore(this.hitsToKill * this.pointsPerHit);
                    InterfaceUpdateEventArgs e = new InterfaceUpdateEventArgs(InterfaceUpdateReasons.ScoreIncreased, this.mhReference.GetCurrentGame().GetHumanPlayer().GetLevelScore().ToString());
                    EventSystem.FireInterfaceUpdate(this.mhReference.GetCurrentGame().GetHumanPlayer(), e);
                }
                else
                {
                    if (this.lastHitByPlayer == Players.FirstPlayer)
                    {
                        this.mhReference.GetCurrentGame().GetHumanPlayer().IncreaseLevelScore(this.hitsToKill * this.pointsPerHit);
                        InterfaceUpdateEventArgs e = new InterfaceUpdateEventArgs(InterfaceUpdateReasons.ScoreIncreased, this.mhReference.GetCurrentGame().GetHumanPlayer().GetLevelScore().ToString());
                        EventSystem.FireInterfaceUpdate(this.mhReference.GetCurrentGame().GetHumanPlayer(), e);
                    }
                    else if (this.lastHitByPlayer == Players.SecondPlayer)
                    {
                        this.mhReference.GetCurrentGame().GetSecondHumanPlayer().IncreaseLevelScore(this.hitsToKill * this.pointsPerHit);
                        InterfaceUpdateEventArgs e = new InterfaceUpdateEventArgs(InterfaceUpdateReasons.ScoreIncreased, this.mhReference.GetCurrentGame().GetSecondHumanPlayer().GetLevelScore().ToString());
                        EventSystem.FireInterfaceUpdate(this.mhReference.GetCurrentGame().GetSecondHumanPlayer(), e);
                    }
                }
            }
        }
        else
        {
            
        }
	}

    protected void SelfDestruct(object sender, ChangeLevelEventArgs e)
    {
        if(this.gameObject != null)
            Destroy(this.gameObject);
    }

    void OnCollisionEnter(Collision c)
    {
        if (Globals.CurrentGameMode == GameModes.SinglePlayer)
        {
            if (c.gameObject.name == "Ball")
            {
                DelayedCollision dc = new DelayedCollision(c.contacts[0], false);
                GameObject.Find("MainHelper").GetComponent<MainHelper>().AddCollisionToQueue(dc);

                ++this.hitsTookPlace;
                this.lastHitByPlayer = Players.FirstPlayer;
            }
        }
        else
        {
            if (c.gameObject.tag == "PlayerBall")
            {
                Players toPlayer = c.gameObject.GetComponent<BallCollisionHandler>().BelongsToPlayer;
                DelayedCollision dc = new DelayedCollision(c.contacts[0], false, c.gameObject, toPlayer);
                GameObject.Find("MainHelper").GetComponent<MainHelper>().AddCollisionToQueue(dc, toPlayer);

                ++this.hitsTookPlace;
                this.lastHitByPlayer = toPlayer;
            }
        }

        if (this.hitsTookPlace >= 1 && this.hitsToKill > 1)
        {
            Texture tex = (Texture)Resources.Load("t_big_crack");
            this.gameObject.GetComponent<Renderer>().material.mainTexture = tex;
        }
    }
}
