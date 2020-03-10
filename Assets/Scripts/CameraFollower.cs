using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    public GameObject FollowingObject;

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = new Vector3(
            FollowingObject.transform.position.x, 
            FollowingObject.transform.position.y, 
            gameObject.transform.position.z
            );
    }
}
