using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponSlotManager : MonoBehaviour
{
    public EnemyManager enemyManager;
    public WeaponItem weaponItem;

    public WeaponSlot equippedSlot;
    public DamageCollider weaponDamageCollider;

    private void Awake()
    {
        enemyManager = GetComponentInParent<EnemyManager>();
        WeaponSlot[] weaponSlots = GetComponentsInChildren<WeaponSlot>();
        foreach (WeaponSlot weapon in weaponSlots)
        {
            equippedSlot = weapon;
        }
    }
    private void Start()
    {
        //if (weaponItem != null)
        //{
        //    LoadWeaponOnSlot(weaponItem);
        //}
    }
    public void LoadWeaponOnSlot(WeaponItem weaponItem)
    {
        equippedSlot.LoadWeaponModel(weaponItem);
        LoadWeaponDamageCollider();
    }

    private void LoadWeaponDamageCollider()
    {
        weaponDamageCollider = equippedSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
    }


    private void OpenWeaponDamageCollider() //在animator里管理开启武器伤害碰撞器
    {
        weaponDamageCollider.EnableDamageCollider();
    }

    private void CloseWeaponDamageCollider() //在animator里管理关闭武器伤害碰撞器
    {
        weaponDamageCollider.DisableDamageCollider();
    }

    private void AnimatorPlaySound(int clipNum) //选择播放的音频
    {

    }

    private void RangeAttack() 
    {
        enemyManager.HandleRangeAttack();
    }

    private void AttackOver()
    {
        enemyManager.isImmuneAttacking = false;
    }

    void WeaponEquip() 
    {
        if (equippedSlot.currentWeaponModel == null)
        {
            LoadWeaponOnSlot(weaponItem);
            enemyManager.isEquipped = true;
        }
        else 
        {
            equippedSlot.UnloadWeapon();
            enemyManager.isEquipped = false;
        }
    }
}
