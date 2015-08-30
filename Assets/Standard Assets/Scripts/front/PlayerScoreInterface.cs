using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerScoreInterface : MonoBehaviour {
    private MainHelper mainHelperReference;
	// Use this for initialization
	void Start () {
        this.mainHelperReference = GameObject.Find("MainHelper").GetComponent<MainHelper>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void UpdateUserScoreOnScreen(int score)
    {  
        this.gameObject.guiText.text = score.ToString();
    }
}
