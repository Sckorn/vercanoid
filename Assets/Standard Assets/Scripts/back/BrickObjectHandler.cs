using UnityEngine;
using System.Collections;

public class BrickObjectHandler : MonoBehaviour {
    private int hitsToKill;
    private int hitsTookPlace = 0;
    private GridCellCoords coordinates;
    private Color[] choosedColors = { Color.black, Color.blue, Color.cyan, Color.gray, Color.green,Color.red, Color.yellow};
    public int pointsPerHit = 10;

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

        gameObject.renderer.material.color = this.choosedColors[(int)Random.Range(0.0f, (float)this.choosedColors.Length)];
	}
	
	// Update is called once per frame
	void Update () {
        if (this.hitsTookPlace == this.hitsToKill)
        {
            Destroy(gameObject);
            GameObject.Find("MainHelper").GetComponent<MainHelper>().GetCurrentGame().GetCurrentField().BrickDestroyed(this.coordinates);
            GameObject.Find("MainHelper").GetComponent<MainHelper>().GetCurrentGame().GetHumanPlayer().IncreaseLevelScore(this.hitsToKill * this.pointsPerHit);
            GameObject.Find("MainHelper").GetComponent<MainHelper>().GetCurrentGame().UpdateUserScoreOnScreen();
        }
	}

    void OnCollisionEnter(Collision c)
    {
        if (this.hitsTookPlace == this.hitsToKill)
        {
            if (c.gameObject.name == "Ball")
            {
                GameObject.Find("Platform").GetComponent<PlatformMover>().LaunchBall(c.contacts[0], false);
            }
        }
        else
        {
            if (c.gameObject.name == "Ball")
            {
                GameObject.Find("Platform").GetComponent<PlatformMover>().LaunchBall(c.contacts[0], false);
                ++this.hitsTookPlace;
            }
        }
    }
}
