using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestFlyingObj : MonoBehaviour
{
    public FlyingObj Template;
    public Transform Target;
    public Transform ini;

    private float m_Timer = 1f;
    private float m_Time;

    private void Update()
    {
        m_Time += Time.deltaTime;
        if (m_Time >= m_Timer)
        {
            m_Time = 0;
            var obj = Instantiate(Template, ini);
            obj.transform.SetParent(null);
            obj.gameObject.SetActive(true);
            obj.StartFlyingObj(Target);
        }
    }
}
