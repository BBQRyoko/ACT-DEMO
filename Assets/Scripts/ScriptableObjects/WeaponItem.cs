using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapon Item")]
public class WeaponItem : Item
{
    public GameObject modelPrefab;
    public bool isEquipped;

    public RuntimeAnimatorController weaponAnimatorController;

    [Header("普通连招")]
    public Skill[] regularSkills;

    [Header("特殊连招")]
    public Skill[] specialSkills;

    [Header("切换攻击")]
    public Skill[] transSkills;

    [Header("处决")]
    public Skill[] executionSkill;

    [Header("跳劈")]
    public Skill[] springAttack;
}
