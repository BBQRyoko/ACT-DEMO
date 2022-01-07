using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 飞行道具
/// </summary>
public class FlyingObj : MonoBehaviour
{
    /// <summary>
    /// 转向速度
    /// </summary>
    [SerializeField] private float m_RotateSpeed;
    /// <summary>
    /// 转弯时的减速
    /// </summary>
    [SerializeField] private float m_SlowDownSpeed;
    /// <summary>
    /// 转弯时最小速度
    /// </summary>
    [SerializeField] private float m_MinSpeed;
    /// <summary>
    /// 加速度
    /// </summary>
    [SerializeField] private float m_Accelerated;
    /// <summary>
    /// 最大移速
    /// </summary>
    [SerializeField] private float m_MaxSpeed;
    /// <summary>
    /// 追踪时间
    /// </summary>
    [SerializeField] private float m_TraceTime;
    /// <summary>
    /// 生命周期
    /// </summary>
    [SerializeField] private float m_LifeTime;

    [Header("方便调试，不要手动修改")]
    /// <summary>
    /// 追踪目标
    /// </summary>
    [SerializeField] private Transform m_TraceTarget;
    [SerializeField] private FlyingMode m_Mode;
    [SerializeField] private bool m_Working;
    [SerializeField] private float m_Speed;
    //private FlyingObjParams m_DefaultArg;
    private System.Func<Collider, FlyingObj, string, bool> m_TriggerCallback;

    private void Awake()
    {
        m_TriggerCallback = DefaultTriggerCondition;
    }

    private void Update()
    {
        if (m_Working == false) return;
        float timeDelta = Time.deltaTime;
        TraceProcess(timeDelta);
        MoveProcess(timeDelta);
        CalcLifeTime(timeDelta);
    }

    /// <summary>
    /// 处理追踪
    /// </summary>
    private void TraceProcess(float timeDelta)
    {
        if (m_Mode != FlyingMode.Trace) return;
        var lookAt = m_TraceTarget.position - transform.position;
        //只需要处理旋转就行了
        float angle = Vector3.Angle(transform.forward, lookAt);
        //判断误差
        if (angle > 0.1f)
        {
            var delta = m_RotateSpeed * timeDelta;
            if (delta > angle)
            {
                transform.forward = lookAt;
            }
            else
            {
                var cross = Vector3.Cross(transform.forward, lookAt);
                //旋转
                transform.Rotate(cross, delta, Space.World);
            }
            //旋转时进行减速
            m_Speed -= m_SlowDownSpeed * timeDelta;
            if (m_Speed < m_MinSpeed)
            {
                m_Speed = m_MinSpeed;
            }
        }
        if (m_TraceTime != -1)
        {
            m_TraceTime -= timeDelta;
            if (m_TraceTime <= 0)
            {
                m_Mode = FlyingMode.StraightLine;
            }
        }
    }

    private void MoveProcess(float timeDelta)
    {
        if (m_Speed < m_MaxSpeed)
        {
            m_Speed += m_Accelerated * timeDelta;
        }
        transform.position += transform.forward * m_Speed * timeDelta;
    }

    /// <summary>
    /// 计算lifetime
    /// </summary>
    /// <param name="timeDelta"></param>
    private void CalcLifeTime(float timeDelta)
    {
        if (m_LifeTime == -1) return;
        m_LifeTime -= timeDelta;
        if (m_LifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 启用飞行物，尽量使用带参数的重载，这个方法用于在Inspector上写好了参数的情况
    /// </summary>
    /// <param name="target">追踪目标</param>
    public void StartFlyingObj(Transform target)
    {
        m_Working = true;
        m_Mode = FlyingMode.Trace;
        m_TraceTarget = target;
        transform.LookAt(target);
        m_Speed = m_MinSpeed;
    }

    /// <summary>
    /// 启用飞行物
    /// </summary>
    /// <param name="target">追踪目标</param>
    /// <param name="arg">参数</param>
    public void StartFlyingObj(Transform target, FlyingObjParams arg)
    {
        SetArgs(arg);
        StartFlyingObj(target);
    }

    /// <summary>
    /// 设置参数
    /// </summary>
    private void SetArgs(FlyingObjParams arg)
    {
        m_RotateSpeed = arg.RotateSpeed;
        m_SlowDownSpeed = arg.SlowDownSpeed;
        m_MinSpeed = arg.MinSpeed;
        m_Accelerated = arg.Accelerated;
        m_MaxSpeed = arg.MaxSpeed;
        m_TraceTime = arg.TraceTime;
        if (arg.TriggerCallback == null)
        {
            m_TriggerCallback = DefaultTriggerCondition;
        }
        else
        {
            m_TriggerCallback = arg.TriggerCallback;
        }
        m_LifeTime = arg.LifeTime;
    }

    #region 碰撞检测

    private void OnTriggerEnter(Collider other)
    {
        if (m_Working == false) return;
        //todo，目前还没有Tag参数
        if (m_TriggerCallback(other, this, string.Empty))
        {
            if (other.CompareTag("Player")) 
            {
                Destroy(this.gameObject);
            }
        }
    }

    /// <summary>
    /// 默认碰撞检测条件
    /// </summary>
    /// <param name="trigger"></param>
    /// <param name="obj"></param>
    /// <returns>true表示检测成功，false表示失败</returns>
    private bool DefaultTriggerCondition(Collider trigger, FlyingObj obj, string belongTag)
    {
        //todo, 判断飞行物和碰撞体的tag来判断敌人和玩家
        //if (trigger.CompareTag(belongTag))
        if (trigger.CompareTag("Enemy") && trigger.GetComponent<FlyingObj>() != null)
        {
            return false;
        }
        //此处未对碰到敌对阵营做任何操作，建议自定义方法
        return true;
    }

    #endregion

    /// <summary>
    /// 飞行模式
    /// </summary>
    private enum FlyingMode : byte
    {
        /// <summary>
        /// 追踪模式
        /// </summary>
        Trace,
        /// <summary>
        /// 直线模式
        /// </summary>
        StraightLine,
    }
}
