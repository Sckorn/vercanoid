using UnityEngine;
using System.Collections;

public class MainHelper : MonoBehaviour {
    private Game currentGame;
    private Transform startingBallPosition;
    void Awake()
    {
        this.currentGame = new Game();
        
    }
	
    // Use this for initialization
	void Start () {
        this.startingBallPosition = GameObject.Find("Ball").transform;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public Game GetCurrentGame()
    {
        return this.currentGame;
    }

    public Transform GetInitialBallTransform()
    {
        return this.startingBallPosition;
    }
}
