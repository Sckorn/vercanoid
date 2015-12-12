using UnityEngine;
using System.Collections;

public class VectorsTesting : MonoBehaviour {
    public bool bUseFixedVector = false;
    public bool bShowXCoordinate = false;
	// Use this for initialization
	void Start () {
        gameObject.GetComponent<Rigidbody>().freezeRotation = true;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            Debug.Log("Local Space Forward Direction " + gameObject.transform.forward.ToString());
            Debug.Log("Local to World Forward Direction " + gameObject.transform.TransformDirection(gameObject.transform.forward).ToString());
            Debug.Log("Vector3 Default forward direction " + Vector3.forward.ToString());
            Debug.Log("Vector3 transform direction direction " + gameObject.transform.TransformDirection(Vector3.forward).ToString());
            if (bUseFixedVector)
            {
                Quaternion rot = Quaternion.FromToRotation(gameObject.transform.forward, new Vector3(0.3f, 0.0f, -0.9f));
                gameObject.transform.rotation = rot;
            }

            gameObject.GetComponent<Rigidbody>().AddForce(gameObject.transform.forward * 100.0f);
        }

        if (bShowXCoordinate)
            Debug.LogWarning("Current X " + gameObject.transform.position.x.ToString());
	}

    void OnCollisionEnter(Collision c)
    {
        gameObject.GetComponent<Rigidbody>().freezeRotation = false;
        Debug.Log("Current local forward " + gameObject.transform.forward.ToString());

        Vector3 newDirection = Vector3.Reflect(gameObject.transform.forward, c.contacts[0].normal);
        //newDirection.Normalize();
        Debug.Log("Collision normal " + c.contacts[0].normal.ToString());
        Debug.Log("Contact points number " + c.contacts.Length.ToString());
       
        Debug.Log("Reflected direction Vector " + newDirection.ToString());
        Quaternion newRotation = Quaternion.FromToRotation(gameObject.transform.forward, newDirection);
        Debug.Log("New forward rotation " + newRotation.eulerAngles.ToString());
        Debug.Log("Current local forward " + gameObject.transform.forward.ToString());
        //gameObject.transform.rotation = Quaternion.identity;
        float valueToRotate = 0.0f;
        if (gameObject.transform.rotation.x != 0.0f)
        {
            valueToRotate = gameObject.transform.rotation.x * -1;
        }

        //gameObject.transform.Rotate(valueToRotate, -newRotation.eulerAngles.y, -newRotation.eulerAngles.z);
        gameObject.transform.rotation = Quaternion.LookRotation(newDirection);
        gameObject.transform.localEulerAngles = new Vector3(0.0f, gameObject.transform.rotation.eulerAngles.y, gameObject.transform.rotation.eulerAngles.z);
        
        Debug.Log("New forward rotation " + gameObject.transform.rotation.eulerAngles.ToString());
        //gameObject.transform.Rotate(gameObject.transform.up, gameObject.transform.rotation.eulerAngles.y - 60.0f);
        if (c.gameObject.name == "Cube")
        {
            gameObject.transform.Rotate(0.0f, -20.0f, 0.0f);
        }
        Debug.Log("New forward rotation " + gameObject.transform.rotation.eulerAngles.ToString());
        Debug.Log("New forward direction " + gameObject.transform.forward.ToString());
        
        gameObject.GetComponent<Rigidbody>().AddForce(gameObject.transform.forward * 100.0f);
        gameObject.GetComponent<Rigidbody>().freezeRotation = true;
    }
}
