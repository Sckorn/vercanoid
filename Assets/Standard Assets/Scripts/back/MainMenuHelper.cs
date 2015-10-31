using UnityEngine;
using System.Collections;

public class MainMenuHelper : MonoBehaviour {

	// Use this for initialization
	void Start () {
        RectTransform rt = GameObject.Find("HeaderPanel").GetComponentInChildren<RectTransform>();
        Debug.Log(rt.sizeDelta.x.ToString() + " " + rt.sizeDelta.y.ToString());
        float quarterOfHeight = Screen.height / 4;
        rt.sizeDelta = new Vector2(Screen.width, quarterOfHeight);

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void StartSinglePlayer()
    {
        Application.LoadLevel(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
