using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    CharacterManager characterManager;

    public int maxHealth;
    public int currHealth;

    public float currStamina;
    [SerializeField] protected float maxStamina = 100;
    [SerializeField] protected float staminaRegen = 5;
}
