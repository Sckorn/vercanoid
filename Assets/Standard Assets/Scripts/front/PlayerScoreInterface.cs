using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerScoreInterface : MonoBehaviour {
    private MainHelper mainHelperReference;
	// Use this for initialization
	void Start () {
        if (Globals.CurrentGameMode == GameModes.SinglePlayer)
        {
            Debug.Log("Single player ?");
            Debug.Log(Globals.CurrentGameMode.ToString());
            this.mainHelperReference = GameObject.Find("MainHelper").GetComponent<MainHelper>();
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void UpdateUserScoreOnScreen(int score)
    {  
        this.gameObject.GetComponent<GUIText>().text = score.ToString();
    }
}
