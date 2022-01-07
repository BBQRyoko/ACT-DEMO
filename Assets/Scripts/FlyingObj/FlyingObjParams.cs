using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 使用飞行物体的参数
/// </summary>
public struct FlyingObjParams
{
    /// <summary>
    /// 旋转速度
    /// </summary>
    public float RotateSpeed { get; }
    /// <summary>
    /// 旋转时减速速度
    /// </summary>
    public float SlowDownSpeed { get; }
    /// <summary>
    /// 物体最小速度（也是初始速度）
    /// </summary>
    public float MinSpeed { get; }
    /// <summary>
    /// 加速度
    /// </summary>
    public float Accelerated { get; }
    /// <summary>
    /// 物体最大速度
    /// </summary>
    public float MaxSpeed { get; }
    /// <summary>
    /// 追踪时间，当等于-1是无限追踪
    /// </summary>
    public float TraceTime { get; }
    /// <summary>
    /// 存活时间，当等于-1时无限存活
    /// </summary>
    public float LifeTime { get; }
    /// <summary>
    /// 碰撞时的回调，参数分别为:Collider 碰撞目标 FlyingObj 飞行物 string 飞行物所属单位的Tag（目前没用） bool 返回值，true为检测成功，false为检测失败
    /// <para>返回true时会销毁物体，false时不会</para>
    /// </summary>
    public System.Func<Collider, FlyingObj, string, bool> TriggerCallback { get; }
}
