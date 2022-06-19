using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    public Slider slider;
    public Slider effectSlider;
    private void Start()
    {
        //slider = GetComponent<Slider>();
    }
    public void SetMaxStamina(float maxStamina)
    {
        slider.maxValue = maxStamina;
        slider.value = maxStamina;
        if (effectSlider) 
        {
            effectSlider.maxValue = maxStamina;
            effectSlider.value = maxStamina;
        }
    }
    public void SetCurrentStamina(float currStamina)
    {
        slider.value = currStamina;
        if (effectSlider != null) 
        {
            if (effectSlider.value >= slider.value)
            {
                effectSlider.value -= 70f * Time.deltaTime;
            }
            if (effectSlider.value < slider.value)
            {
                effectSlider.value = slider.value;
            }
        }
    }
}
