using UnityEngine;
using System.Collections;

public class Morpher : MonoBehaviour {
    public GameObject brickPrefab;

    public GameObject firstPlayerBallReference = null;
    public GameObject secondPlayerBallReference = null;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public GameObject MorphBrick(int _x, int _y)
    {
        return (GameObject)Instantiate(this.brickPrefab, new Vector3(14.0f - _x, 0.25f, 11.0f - _y / 3.85f), Quaternion.identity);
    }

    public GameObject MorphBrick(int _x, int _y, char brickCharType, int hitsToKill = 1)
    {
        GameObject go = (GameObject)Instantiate(this.brickPrefab, new Vector3(18.0f - _x, 0.25f, 13.0f - _y / 3.85f), Quaternion.identity);
        go.GetComponent<BrickObjectHandler>().HitsToKill = hitsToKill;
        return go;
    }

    public void DestroyBrick(GameObject go)
    {
        Destroy(go);
    }

    public void KillTheBall()
    {
        GameObject ballObj = GameObject.Find("Ball");
        Destroy(ballObj);
    }

    public void KillBallIndicator(GameObject indicatorReference)
    {
        Destroy(indicatorReference);
    }
}
