using System.Collections;
using System.Security.Cryptography;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    public GameObject followingObject;
    public bool isRectTransformMode;
    public Player player;
    public Vector3 offset = Vector3.zero;
    public float smoothRate = 0.05f;
    public float betweenRate = 0.3f;
    Vector3 destination;

    void FixedUpdate()
    {
        if (isRectTransformMode)
        {
            destination = new Vector2(
                followingObject.GetComponent<RectTransform>().anchoredPosition.x,
                gameObject.GetComponent<RectTransform>().anchoredPosition.y
                );

            Vector2 smoothDestination = Vector2.Lerp(
                gameObject.GetComponent<RectTransform>().anchoredPosition,
                destination,
                smoothRate);
            //更新位置
            gameObject.GetComponent<RectTransform>().anchoredPosition = smoothDestination + (Vector2)offset;
        }
        else
        {
            //获取跟踪目标的位置
            if (followingObject == player.gameObject && player.enemy)
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
            gameObject.transform.position = smoothDestination + offset;
        }
    }

    public void FollowSomething(GameObject target)
    {
        followingObject = target;
    }

    public void FollowSomething(GameObject target, float followingTime)
    {
        StartCoroutine("FollowSomethingForATime", (target, followingTime));
    }

    public void FollowEnemy(float followingTime)
    {
        StartCoroutine("FollowEnemyCrt", followingTime);
    }

    IEnumerator FollowSomethingForATime((GameObject, float) pair)
    {
        var originTarget = followingObject;
        followingObject = pair.Item1;
        yield return new WaitForSeconds(pair.Item2);
        followingObject = originTarget;

    }
    IEnumerator FollowEnemyCrt(float followingTime)
    {
        var originBetweenRate = betweenRate;
        betweenRate = 0.9f;
        yield return new WaitForSeconds(followingTime);
        betweenRate = originBetweenRate;
    }
}
