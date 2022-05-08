using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaijiDurationBar : MonoBehaviour
{
    public Slider slider;
    private void Start()
    {
        //slider = GetComponent<Slider>();
    }
    public void SetMaxTime(float maxTime)
    {
        slider.maxValue = maxTime;
        slider.value = maxTime;
    }
    public void SetCurrentTime(float currTime)
    {
        slider.value = currTime;
    }
}
