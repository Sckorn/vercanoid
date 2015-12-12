using UnityEngine;
using System.Collections;

public class ReflectionWallsHandler : MonoBehaviour
{
    public GameObject collisionTarget;

    #region multiplayer_objects_references
    public GameObject firstPlayerCollisionTarget;
    public GameObject secondPlayerCollisionTarget;
    #endregion
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
        if (Globals.CurrentGameMode == GameModes.SinglePlayer)
        {
            if (c.gameObject.name == this.collisionTarget.name)
            {
                if (this.gameObject.tag == "wall")
                    GameObject.Find("Platform").GetComponent<PlatformMover>().LaunchBall(c.contacts[0], false, Vector3.right);
                else if (this.gameObject.tag == "frontWall")
                    GameObject.Find("Platform").GetComponent<PlatformMover>().LaunchBall(c.contacts[0], false, Vector3.back);
                else
                    GameObject.Find("Platform").GetComponent<PlatformMover>().LaunchBall(c.contacts[0], false);
            }
        }
        else
        {
            if (c.gameObject.name == this.firstPlayerCollisionTarget.name || c.gameObject.name == this.secondPlayerCollisionTarget.name)
            {
                string requiredPlatform = string.Empty;
                Vector3 sideReflectionVector = Vector3.zero;
                Vector3 frontReflectionVector = Vector3.zero;
                if (c.gameObject.name == this.firstPlayerCollisionTarget.name)
                {
                    requiredPlatform = "FirstPlayerPlatform";
                    sideReflectionVector = Vector3.right;
                    frontReflectionVector = Vector3.forward;
                }
                else if(c.gameObject.name == this.secondPlayerCollisionTarget.name)
                {
                    requiredPlatform = "SecondPlayerPlatform";
                    sideReflectionVector = Vector3.right;
                    frontReflectionVector = Vector3.forward;
                }

                if (gameObject.name == "LeftWall")
                    sideReflectionVector = Vector3.right;
                else
                    sideReflectionVector = Vector3.left;

                if (this.gameObject.tag == "wall")
                    GameObject.Find(requiredPlatform).GetComponent<PlatformMover>().LaunchBall(c.contacts[0], false, c.gameObject.GetComponent<BallCollisionHandler>().BelongsToPlayer);
                else
                    GameObject.Find(requiredPlatform).GetComponent<PlatformMover>().LaunchBall(c.contacts[0], false);
            }
        }
    }
}
