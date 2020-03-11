using DanmakU;
using UnityEngine;

public class WhoseDanmaku : MonoBehaviour
{
    public static bool IsMyDanmaku(Danmaku danmaku, DanmakuEmitter myEmitter)
        => myEmitter.Speed.GetValue() == danmaku.Speed;
}
