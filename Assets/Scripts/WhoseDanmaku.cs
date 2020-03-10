using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DanmakU;

public class WhoseDanmaku : MonoBehaviour
{
    public static bool IsMyDanmaku(Danmaku danmaku, DanmakuEmitter myEmitter) 
        => myEmitter.Speed.GetValue() == danmaku.Speed;
}
