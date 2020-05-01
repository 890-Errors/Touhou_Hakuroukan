using System.Security.Cryptography;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    public GameObject followingObject;
    public Player player;
    public float smoothRate = 0.05f;
    public float betweenRate = 0.3f;
    Vector3 destination;

    void FixedUpdate()
    {
        //获取跟踪目标的位置
        if (player.gameObject == followingObject && player.enemy)
        {
            destination = Vector3.Lerp(
                player.gameObject.transform.position,
                player.enemy.transform.position,
                betweenRate);
            destination.z = gameObject.transform.position.z;
        }
        else
        {
            destination = new Vector3(
            followingObject.transform.position.x,
            followingObject.transform.position.y,
            gameObject.transform.position.z
            );
        }

        //线性插值平滑摄像机移动
        Vector3 smoothDestination = Vector3.Lerp(
            gameObject.transform.position, 
            destination, 
            smoothRate);
        //更新摄像机位置
        gameObject.transform.position = smoothDestination;

    }
}
