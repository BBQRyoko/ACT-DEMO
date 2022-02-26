using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public Transform lockOnTransform;

    public bool isRotatingWithRootMotion;
    public bool canRotate;

    public bool isWeak;
    public float weakTimer;
    public bool isDead;
}
