using UnityEngine;

public class HitboxLine : MonoBehaviour
{
    //static HitboxLine S;    //单例模式

    //public float minDist = .1f;

    //public bool __________________;

    //public LineRenderer line;
    //private GameObject _poi;
    //public List<Vector3> points;

    //private void Awake()
    //{
    //    S = this;
    //    line = GetComponent<LineRenderer>();
    //    line.enabled = false;   //一开始无需启用
    //    points = new List<Vector3>();
    //}

    //public GameObject poi
    //{
    //    get { return _poi; }
    //    set
    //    {
    //        _poi = value;
    //        if (_poi != null)
    //        {
    //            line.enabled = false;
    //            points = new List<Vector3>();
    //            AddPoint();
    //        }
    //    }
    //}

    //public void Clear()
    //{
    //    _poi = null;
    //    line.enabled = false;
    //    points = new List<Vector3>();

    //}

    //public void AddPoint()
    //{
    //    Vector3 pt = _poi.transform.position;
    //    //点的位置与最后的记录相隔不远，则忽略之
    //    if (points.Count > 0 && (pt - lastPoint).magnitude < minDist)
    //    {
    //        return;
    //    }
    //    if (points.Count == 0)
    //    {
    //        //如果当前是发射点
    //        Vector3 launchPos = SlingShot.S.launchPoint.transform.position;
    //        Vector3 launchPosDiff = pt - launchPos;
    //        //……则添加一根线条辅助瞄准
    //        points.Add(pt + launchPosDiff);
    //        points.Add(pt);
    //        line.positionCount = 2;
    //        //设置前两个点
    //        line.SetPosition(0, points[0]);
    //        line.SetPosition(1, points[1]);
    //        //启用LineRenderer
    //        line.enabled = true;
    //    }
    //    else
    //    {
    //        //正常的添加点操作
    //        points.Add(pt);
    //        line.positionCount = points.Count;
    //        line.SetPosition(points.Count - 1, lastPoint);
    //        line.enabled = true;
    //    }
    //}

    ////返回最近添加的点的位置(只读属性)
    //public Vector3 lastPoint
    //{
    //    get
    //    {
    //        if (points == null)
    //        {
    //            return Vector3.zero;
    //        }
    //        return points[points.Count - 1];
    //    }
    //}

    //private void FixedUpdate()
    //{
    //    //如果兴趣点不存在则找出一个
    //    if (poi == null)
    //    {
    //        if (FollowCam.S.poi.tag == "Projectile")
    //        {
    //            poi = FollowCam.S.poi;
    //        }
    //        else
    //        {
    //            return;     //未找到兴趣点则返回
    //        }
    //    }
    //    AddPoint();
    //    if (poi.rigidpody.IsSleeping())
    //    {
    //        poi = null;
    //    }
    //}
}
