using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PanelResizer : MonoBehaviour {
    public GameObject goParentCanvas;
	// Use this for initialization
	void Start () {
        RectTransform rt = this.goParentCanvas.GetComponent<RectTransform>();
        Debug.LogError(rt.rect.width.ToString());
        this.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(rt.rect.width, rt.rect.height);
	}
	
	// Update is called once per frame
	void Update () {
        
	}
}
