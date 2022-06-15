using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    CharacterManager characterManager;

    public float maxHealth;
    public float currHealth;
    public Transform eyePos;

    public float currStamina;
    public float maxStamina = 100;
    public float staminaRegen = 5;

    private void Update()
    {
        if (currHealth >= maxHealth) 
        {
            currHealth = maxHealth;
        }

        if (currStamina >= maxStamina) 
        {
            currStamina = maxStamina;  
        }
    }
}
