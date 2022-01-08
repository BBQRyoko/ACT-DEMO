using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Enemy Actions/SpecialCondition")]
public class SpecialCondition : ScriptableObject
{
    public float range;
    public int equationType; // 0 is ==, 1 is >=, 2 is <=
    public int attackIndex;
}
