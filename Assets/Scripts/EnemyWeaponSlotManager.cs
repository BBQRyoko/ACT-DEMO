using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponSlotManager : MonoBehaviour
{
    public EnemyManager enemyManager;
    public WeaponItem weaponItem;
    [SerializeField] GameObject UnequipWeapon;
    WeaponSlot weaponSlot;

    [SerializeField] float distanceToTarget;
    [SerializeField] bool distanceCheck;
    public WeaponSlot equippedSlot;
    public DamageCollider weaponDamageCollider;
    public DamageCollider kickDamagerCollider;
    public ProjectileDamager flyingObjectDamager;
    [SerializeField] FlyingObj flyingObj;
    public ProjectileDamager flyingObjectDamager2;
    [SerializeField] FlyingObj flyingObj2;

    [Header("FinalBossOnly")]
    public ProjectileDamager specialProjectilDamager;


    private void Awake()
    {
        enemyManager = GetComponentInParent<EnemyManager>();
        weaponSlot = GetComponentInChildren<WeaponSlot>();
        WeaponSlot[] weaponSlots = GetComponentsInChildren<WeaponSlot>();
        foreach (WeaponSlot weapon in weaponSlots)
        {
            equippedSlot = weapon;
        }
    }
    private void Start()
    {
        if (flyingObj) flyingObjectDamager = flyingObj.GetComponentInChildren<ProjectileDamager>();
        if (flyingObj2) flyingObjectDamager2 = flyingObj2.GetComponent<ProjectileDamager>();
        //if (weaponItem != null)
        //{
        //    LoadWeaponOnSlot(weaponItem);
        //}
    }
    private void Update()
    {
        ComboCheck();
        if (enemyManager.phaseChanged) { FinalBossProjectileChange(); }
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
    public void CloseWeaponDamageCollider() //在animator里管理关闭武器伤害碰撞器
    {
        if (weaponDamageCollider) 
        {
            weaponDamageCollider.DisableDamageCollider();
        }
    }
    private void OpenKickDamageCollider() //在animator里管理开启武器伤害碰撞器
    {
        kickDamagerCollider.EnableDamageCollider();
    }
    private void CloseKickDamageCollider() //在animator里管理关闭武器伤害碰撞器
    {
        kickDamagerCollider.DisableDamageCollider();
    }
    private void CanComboDistanceCheck() 
    {
        if (distanceCheck)
        {
            distanceCheck = false;
        }
        else 
        {
            distanceCheck = true;
        }
    }
    private void ComboCheck() 
    {
        if (distanceCheck && enemyManager.curTarget) 
        {
            distanceToTarget = Vector3.Distance(enemyManager.curTarget.transform.position, enemyManager.transform.position);
            Animator animator = GetComponent<Animator>();
            if (distanceToTarget <= enemyManager.maxCombatRange)
            {
                animator.SetTrigger("canCombo");
                distanceCheck = false;
                gameObject.GetComponent<EnemyAnimatorManager>().animatorSpeed = 1;
            }
        }
    }
    private void RangeAttack(EnemyAttackAction attackAction) 
    {
        flyingObjectDamager.curDamage = attackAction.damagePoint;
        flyingObjectDamager.isHeavy = attackAction.isHeavyAttack;
        flyingObj.m_MaxSpeed = attackAction.maxSpeed;
        enemyManager.HandleRangeAttack();
    }
    private void RangeAttack2(EnemyAttackAction attackAction)
    {
        flyingObjectDamager2.curDamage = attackAction.damagePoint;
        flyingObjectDamager2.isHeavy = attackAction.isHeavyAttack;
        flyingObj.m_MaxSpeed = attackAction.maxSpeed;
        enemyManager.HandleRangeAttack2();
    }
    private void AttackOver()
    {
        enemyManager.isImmuneAttacking = false;
    }
    public void WeaponEquip() 
    {
        if (equippedSlot.currentWeaponModel == null)
        {
            LoadWeaponOnSlot(weaponItem);
            enemyManager.isEquipped = true;
            if (UnequipWeapon != null) 
            {
                UnequipWeapon.SetActive(false);
            }
        }
        else 
        {
            equippedSlot.UnloadWeapon();
            weaponDamageCollider = null;
            Destroy(weaponSlot.currentWeaponModel.gameObject);
            enemyManager.isEquipped = false;
            if (UnequipWeapon != null)
            {
                UnequipWeapon.SetActive(true);
            }
        }
    }
    void FinalBossProjectileChange() 
    {
        if (flyingObjectDamager != specialProjectilDamager) 
        {
            flyingObjectDamager = specialProjectilDamager;
            flyingObj = specialProjectilDamager.GetComponent<FlyingObj>();
            enemyManager.arrow = specialProjectilDamager.GetComponent<FlyingObj>();
        }
    }
}
