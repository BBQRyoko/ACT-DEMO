using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public Transform lockOnTransform;

    public bool isRotatingWithRootMotion;
    public bool canRotate;

    public bool isDead;

    //Debuff
    [Header("Debuff")]
    public int debuff_Index; // (0=nothing, 1 = stuned, 2 = burning)
    public bool isStunned;
    public float stunTimer;
    public bool isToronadoCovered;
    public bool isParryBreak;
}
