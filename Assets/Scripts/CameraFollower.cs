using System.Security.Cryptography;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    public GameObject FollowingObject;
    public float easing = 0.05f;

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 destination = new Vector3(
            FollowingObject.transform.position.x,
            FollowingObject.transform.position.y,
            gameObject.transform.position.z
            );
        Vector3 smoothDestination = Vector3.Lerp(gameObject.transform.position, destination, easing);
        gameObject.transform.position = smoothDestination;

    }
}
