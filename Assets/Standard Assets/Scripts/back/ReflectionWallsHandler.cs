using UnityEngine;
using System.Collections;

public class ReflectionWallsHandler : MonoBehaviour
{
    public GameObject collisionTarget;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.name == this.collisionTarget.name)
        {
            if(this.gameObject.tag == "wall")
                GameObject.Find("Platform").GetComponent<PlatformMover>().LaunchBall(c.contacts[0], false, Vector3.right);
            else
                GameObject.Find("Platform").GetComponent<PlatformMover>().LaunchBall(c.contacts[0], false);
        }
    }
}
