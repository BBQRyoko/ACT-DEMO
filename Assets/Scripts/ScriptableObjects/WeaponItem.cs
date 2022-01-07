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

    [Header("武器技能")]
    public Skill[] weaponAbilities;
}
