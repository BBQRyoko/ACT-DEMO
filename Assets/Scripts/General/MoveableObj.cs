using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveableObj : MonoBehaviour
{
    [Header("路线位置点")]
	public List<Vector3> PosList;
    [Header("是否形成闭环")]
    public bool IsCircle;
    [Header("触发检测层")]
    public LayerMask DetectionLayer;
    [Header("速度")]
    public float Speed;
    [Header("停止时间")]
    public float StopTime;
    [Header("下面的参数用来调试的")]
    /// <summary>
    /// 当前路线索引
    /// </summary>
    [SerializeField] private int m_CurrIndex;
    /// <summary>
    /// 初始位置
    /// </summary>
    [SerializeField] private Vector3 m_InitPos;
    [SerializeField] private float m_Timer;
    [SerializeField] private float m_NowTime;
    /// <summary>
    /// 路径起点
    /// </summary>
    [SerializeField] private Vector3 m_PathStart;
    /// <summary>
    /// 路径结束点
    /// </summary>
    [SerializeField] private Vector3 m_PathEnd;
    /// <summary>
    /// 转换路线的索引递增变量
    /// </summary>
    [SerializeField] private int m_Delta;
    [SerializeField] private ObjState m_State;
    //[SerializeField] private 

    

    private void Awake()
    {
        m_InitPos = transform.position;
        m_CurrIndex = -1;
        m_Timer = 0;
        m_NowTime = 0;
        m_Delta = 1;
        m_State = ObjState.Redirect;
        StartCoroutine(IETransformPath());
    }

    private void Update()
    {
        if (PosList == null || PosList.Count == 0) return;
        MoveObj(Time.deltaTime);
        TransformPath();
    }

    /// <summary>
    /// 转换路线
    /// </summary>
    private void TransformPath()
    {
        if (m_State != ObjState.Stop) return;
        StartCoroutine(IETransformPath());
    }

    private IEnumerator IETransformPath()
    {
        m_State = ObjState.Redirect;
        m_PathStart = m_PathEnd;
        if (m_CurrIndex == PosList.Count - 1)
        {
            if (IsCircle)
            {
                if (m_Delta > 0)
                {
                    m_PathEnd = m_InitPos;
                    m_CurrIndex = -1;
                }
                else
                {
                    m_CurrIndex += m_Delta;
                }
            }
            else
            {
                m_Timer = 0;
                m_State = ObjState.Stop;
                yield break;
            }
        }
        else
        {
            m_CurrIndex += m_Delta;
            if (m_CurrIndex < -1)
            {
                m_CurrIndex = PosList.Count - 1;
            }
            else if (m_CurrIndex > PosList.Count - 1)
            {
                m_CurrIndex = -1;
            }
        }
        if (m_CurrIndex == -1)
        {
            m_PathEnd = m_InitPos;
        }
        else
        {
            m_PathEnd = PosList[m_CurrIndex];
        }
        if (Speed != 0)
        {
            m_Timer = Vector3.Distance(m_PathStart, m_PathEnd) / Speed;
            //m_NowTime = 0;
        }
        yield return new WaitForSeconds(StopTime);
        transform.LookAt(m_PathEnd);
        m_State = ObjState.Move;
    }

    /// <summary>
    /// 移动物体
    /// </summary>
    private void MoveObj(float time)
    {
        if (m_State != ObjState.Move) return;
        m_NowTime += time;
        transform.position = Vector3.Lerp(m_PathStart, m_PathEnd, m_NowTime / m_Timer);
        if (m_NowTime >= m_Timer)
        {
            m_State = ObjState.Stop;
            m_NowTime = 0;
        }
    }

#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        Vector3 initPos = Application.isPlaying ? m_InitPos : transform.position;
        if (PosList == null || PosList.Count == 0) return;
        Gizmos.color = Color.cyan;
        Vector3 s = initPos;
        Vector3 e;
        for(int i = 0; i < PosList.Count; i++)
        {
            e = PosList[i];
            Gizmos.DrawSphere(e, 0.2f);
            Gizmos.DrawLine(s, e);
            s = e;
        }
        if (IsCircle && PosList.Count > 1)
        {
            e = initPos;
            Gizmos.DrawSphere(e, 0.2f);
            Gizmos.DrawLine(s, e);
        }
    }
#endif

    //private void OnTriggerEnter(Collider other)
    //{
    //    //Debug.LogError("111");
    //    //检测到了触发
    //    //if ((other.gameObject.layer & DetectionLayer) == other.gameObject.layer)
    //    if (other.gameObject.CompareTag("Player"))
    //    {
    //        Debug.LogError("22");
    //        //进行停止并反向回走
    //        m_NowTime = m_Timer - m_NowTime;
    //        m_State = ObjState.Stop;
    //        m_Delta *= -1;
    //        TransformPath();
    //    }
    //}

    private void OnCollisionEnter(Collision collision)
    {
        //检测到了触发
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.LogError("22");
            //进行停止并反向回走
            m_NowTime = m_Timer - m_NowTime;
            m_State = ObjState.Stop;
            m_Delta *= -1;
            TransformPath();
        }
    }

    /// <summary>
    /// 当前物体状态
    /// </summary>
    private enum ObjState
    {
        /// <summary>
        /// 重定向中
        /// </summary>
        Redirect,
        /// <summary>
        /// 移动中
        /// </summary>
        Move,
        /// <summary>
        /// 停止
        /// </summary>
        Stop,
    }
}
