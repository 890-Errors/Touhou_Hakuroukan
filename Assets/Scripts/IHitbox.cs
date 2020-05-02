using UnityEngine;
using DanmakU;

public interface IHitbox
{
    Collider2D Collider2D { get; set; }
    DanmakuCollider DanmakuCollider { get; set; }
    IHealthPoint ParentController { get; set; }

    void OnDanmakuCollision(DanmakuCollisionList danmakuCollisions);

}
