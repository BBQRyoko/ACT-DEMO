using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Enemy Actions/SpecialCondition")]
public class SpecialCondition : EnemyAttackAction
{
    public enum conditionType { 距离型, 玩家攻击型, 玩家蓄力硬直型, 玩家防御型, 飞行道具型};
    public conditionType condition;

    [Header("特殊条件")]
    [Header("距离专用")]
    public float minRange;
    public float maxRange;

    [Header("攻击专用")]
    public bool canDodge;

}
