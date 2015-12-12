using UnityEngine;
using System.Collections;

public class HotSeatHelper : MonoBehaviour {
    private InterfaceUpdater InterfaceUpdaterRef = null;
    private GameObject MainCameraRef = null;
    private Game currentGame;
	// Use this for initialization

    void OnEnable() {
        //this.currentGame = new Game(GameModes.Versus);
    }

    void Awake()
    {
        Globals.SetGameMode(GameModes.Versus);
    }
	
    void Start () {
        this.InterfaceUpdaterRef = GameObject.Find("InterfaceUpdater").GetComponent<InterfaceUpdater>();
        this.InterfaceUpdaterRef.ChangeColorsByTagsHotseat();
	}
	
	// Update is called once per frame
	void Update () {
        //Debug.Log("Current game mode " + Globals.CurrentGameMode.ToString());
	}
}
