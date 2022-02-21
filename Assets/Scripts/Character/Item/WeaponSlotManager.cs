using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSlotManager : MonoBehaviour
{
    PlayerManager playerManager;
    [SerializeField] Sample_VFX sample_VFX;

    [SerializeField] WeaponSlot mainWeapon_Unequipped;
    public WeaponSlot[] weaponSlots = new WeaponSlot[2];

    public DamageCollider weaponDamageCollider;
    [SerializeField] ParryCollider parryCollider;
    public GameObject mainArmedWeapon;
    [SerializeField] GameObject[] armedWeaponSlot = new GameObject[2];

    private void Awake()
    {
        playerManager = GetComponentInParent<PlayerManager>();
        weaponSlots = GetComponentsInChildren<WeaponSlot>();
        foreach(WeaponSlot weapon in weaponSlots) 
        {
            mainWeapon_Unequipped = weaponSlots[0];
        }
        mainArmedWeapon = armedWeaponSlot[0];
    }
    public void LoadWeaponOnSlot(WeaponItem weaponItem, int index) 
    {
        if (index == 0)
        {
            weaponSlots[0].LoadWeaponModel(weaponItem);
        }
        else 
        {
            weaponSlots[1].LoadWeaponModel(weaponItem);
        }
    }
    public void EquipeWeapon() 
    {
        if (!playerManager.isWeaponEquipped)
        {
            mainWeapon_Unequipped.gameObject.SetActive(true);
            mainArmedWeapon.SetActive(false);
        }
        else 
        {
            mainWeapon_Unequipped.gameObject.SetActive(false);
            mainArmedWeapon.SetActive(true);
        }
    }
    public void WeaponSwitch() 
    {
        if (playerManager.GetComponent<PlayerInventory>().unequippedWeaponItems.Length == 2 && playerManager.weaponSwitchCooldown <=0) 
        {
            if (!playerManager.isAttacking && !playerManager.isInteracting)
            {
                if (!playerManager.isWeaponEquipped) //无装备态
                {
                    if (mainWeapon_Unequipped == weaponSlots[0])
                    {
                        playerManager.GetComponent<PlayerInventory>().currentWeaponIndex = 1;
                        mainWeapon_Unequipped = weaponSlots[1];
                        transform.GetComponent<Animator>().runtimeAnimatorController = playerManager.GetComponent<PlayerInventory>().unequippedWeaponItems[1].weaponAnimatorController;
                        mainArmedWeapon = armedWeaponSlot[1];
                        playerManager.WeaponSwitchTimerSetUp(1f);
                    }
                    else
                    {
                        playerManager.GetComponent<PlayerInventory>().currentWeaponIndex = 0;
                        mainWeapon_Unequipped = weaponSlots[0];
                        transform.GetComponent<Animator>().runtimeAnimatorController = playerManager.GetComponent<PlayerInventory>().unequippedWeaponItems[0].weaponAnimatorController;
                        mainArmedWeapon = armedWeaponSlot[0];
                        playerManager.WeaponSwitchTimerSetUp(1f);
                    }
                }
                else //装备态
                {
                    playerManager.isWeaponEquipped = false;
                    transform.GetComponent<AnimatorManager>().PlayTargetAnimation("WeaponSwitch(Unarm)", true, true);
                    playerManager.isWeaponSwitching = true;
                }
            }
            else if(playerManager.isAttacking) 
            {
                playerManager.isWeaponEquipped = false;
                transform.GetComponent<Animator>().SetTrigger("isWeaponSwitching");
                playerManager.isWeaponSwitching = true;
            }
        }
    }
    private void WeaponSwitchTimerSetup() 
    {
        playerManager.isWeaponSwitching = false;
        playerManager.WeaponSwitchTimerSetUp(3f);
    }
    private void WeaponSwitchAnimatorController() 
    {
        if (!playerManager.isGettingDamage) 
        {
            if (mainWeapon_Unequipped == weaponSlots[0])
            {
                playerManager.GetComponent<PlayerInventory>().currentWeaponIndex = 1;
                playerManager.perfectTimer = 0.75f;
                mainWeapon_Unequipped = weaponSlots[1];
                mainArmedWeapon = armedWeaponSlot[1];
                transform.GetComponent<Animator>().runtimeAnimatorController = playerManager.GetComponent<PlayerInventory>().unequippedWeaponItems[1].weaponAnimatorController;
                transform.GetComponent<AnimatorManager>().PlayTargetAnimation("WeaponSwitch(Equip)", true, true);
                playerManager.isWeaponEquipped = true;
                sample_VFX.baGuaRelated_List[0].Play();
            }
            else
            {
                playerManager.GetComponent<PlayerInventory>().currentWeaponIndex = 0;
                playerManager.perfectTimer = 0.75f;
                mainWeapon_Unequipped = weaponSlots[0];
                mainArmedWeapon = armedWeaponSlot[0];
                transform.GetComponent<Animator>().runtimeAnimatorController = playerManager.GetComponent<PlayerInventory>().unequippedWeaponItems[0].weaponAnimatorController;
                transform.GetComponent<AnimatorManager>().PlayTargetAnimation("WeaponSwitch(Equip)", true, true);
                playerManager.isWeaponEquipped = true;
                sample_VFX.baGuaRelated_List[0].Play();
            }
        }
    }
    #region Handle Weapon's Damage Collider
    private void LoadWeaponDamageCollider() //读取当前所使用的武器
    {
        weaponDamageCollider = mainArmedWeapon.GetComponentInChildren<DamageCollider>();
    }
    private void OpenWeaponDamageCollider() //在动画器中开启对应武器的碰撞器
    {
        weaponDamageCollider.EnableDamageCollider();
    }
    private void OpenParryCollider() //在动画器中开启对应武器的碰撞器
    {
        parryCollider.EnableParryCollider();
    }
    private void OpenVFXCollider (DamageCollider collider) //在动画器中开启对应VFX的碰撞器
    {
        collider.EnableDamageCollider();
    }
    private void CloseWeaponDamageCollider() //在动画器中关闭对应武器的碰撞器
    {
        weaponDamageCollider.DisableDamageCollider();
    }
    private void CloseParryCollider() //在动画器中关闭对应武器的碰撞器
    {
        parryCollider.DisableParryCollider();
    }
    private void PerfectParryOn() 
    {
        parryCollider.PerfectTiming();
    }
    private void CloseVFXCollider(DamageCollider collider) //在动画器中关闭对应VFX的碰撞器
    {
        collider.DisableDamageCollider();
    }
    private void AttackOver() //确定何时提前关闭玩家当前的攻击状态
    {
        playerManager.cantBeInterrupted = false;
    }
    #endregion
}
