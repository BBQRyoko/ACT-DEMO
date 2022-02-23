using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Enemy Actions/Attack Action")]
public class EnemyAttackAction : EnemyAction
{
    public bool isSpecial;
    public bool isFlyingObject;
    public bool isHeavyKick;

    [Header("攻击基础参数")]
    public bool isHeavyAttack;
    public int damagePoint = 10;
    public int attakPriority = 3;
    public float recoveryTime = 2;
    public float independtCooldown = 7;

    public float maxAttackAngle = 35;
    public float minAttackAngle = -35;

    public float minDistanceNeedToAttack = 0;
    public float maxDistanceNeedToAttack = 3;

    public bool isImmune;

    [Header("飞行道具参数")]
    public float maxSpeed = 20f;
}
