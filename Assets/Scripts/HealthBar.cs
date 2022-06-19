using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Slider effectSlider;

    private void Start()
    {
        //slider = GetComponent<Slider>();
    }
    public void SetMaxHealth(float maxHealth) 
    {
        slider.maxValue = maxHealth;
        slider.value = maxHealth;
        effectSlider.maxValue = maxHealth;
        effectSlider.value = maxHealth;
    }
    public void SetCurrentHealth(float currHealth) 
    {
        slider.value = currHealth;
        if (effectSlider.value >= slider.value) 
        {
            effectSlider.value -= 75f * Time.deltaTime;
        }
        if (effectSlider.value < slider.value)
        {
            effectSlider.value = slider.value;
        }
    }
}
