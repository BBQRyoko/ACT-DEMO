using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSlotManager : MonoBehaviour
{
    InputManager inputManager;
    PlayerManager playerManager;
    PlayerAttacker playerAttacker;
    PlayerInventory playerInventory;
    Animator animator;
    [SerializeField] Sample_VFX sample_VFX;

    public WeaponSlot mainWeapon_Unequipped;
    public WeaponSlot[] unequippedWeaponSlots = new WeaponSlot[2];

    public DamageCollider weaponDamageCollider;
    [SerializeField] ParryCollider parryCollider;
    public GameObject mainArmedWeapon;
    public FlyingObj[] arrowObjs; 
    [SerializeField] GameObject[] armedWeaponSlot = new GameObject[4];
    
    [Header("displayOnly")]
    [SerializeField] GameObject arrowPrefab; 
    [SerializeField] GameObject powerArrowPrefab;

    [SerializeField] GameObject greatSwordIcon;
    [SerializeField] GameObject katanaIcon;

    private void Awake()
    {
        inputManager = GetComponentInParent<InputManager>();
        playerManager = GetComponentInParent<PlayerManager>();
        playerAttacker = GetComponentInParent<PlayerAttacker>();
        playerInventory = GetComponentInParent<PlayerInventory>();
        animator = GetComponent<Animator>();
        unequippedWeaponSlots = GetComponentsInChildren<WeaponSlot>();
        foreach(WeaponSlot weapon in unequippedWeaponSlots) 
        {
            mainWeapon_Unequipped = unequippedWeaponSlots[0];
        }
        mainArmedWeapon = armedWeaponSlot[0];
    }
    public void LoadWeaponOnSlot(WeaponItem weaponItem, int index)  //读取未装备的武器模型
    {
        if (index == 0)
        {
            unequippedWeaponSlots[0].LoadWeaponModel(weaponItem);
        }
        else 
        {
            unequippedWeaponSlots[1].LoadWeaponModel(weaponItem);
        }
    }
    public void LoadArrowOnSlot() 
    {
        arrowPrefab.SetActive(true);
    }
    public void LoadPowerArrowOnSlot() 
    {
        arrowPrefab.SetActive(false);
        powerArrowPrefab.SetActive(true);
    }
    public void UnloadArrowOnSlot() 
    {
        arrowPrefab.SetActive(false);
        powerArrowPrefab.SetActive(false);
    }
    public void EquipeWeapon() 
    {
        mainWeapon_Unequipped.gameObject.SetActive(false);
        mainArmedWeapon.SetActive(true);
    }
    public void WeaponSwitch() 
    {
        if (playerManager.GetComponent<PlayerInventory>().unequippedWeaponItems[1]!=null && playerManager.weaponSwitchCooldown <=0) 
        {
            if (!playerManager.isAttacking && !playerManager.isInteracting && !playerManager.cantBeInterrupted && !playerManager.canTransAttack)
            {
                GetComponentInChildren<WeaponSlotManager>().mainArmedWeapon.SetActive(false);
                GetComponentInChildren<WeaponSlotManager>().mainWeapon_Unequipped.gameObject.SetActive(true);
                WeaponSwitchAnimatorController();
                playerManager.isWeaponSwitching = true;
                UnloadArrowOnSlot();
            }
            else if(playerManager.canTransAttack) //可触发条件
            {
                GetComponentInChildren<WeaponSlotManager>().mainArmedWeapon.SetActive(false);
                GetComponentInChildren<WeaponSlotManager>().mainWeapon_Unequipped.gameObject.SetActive(true);
                AttackSwitchAnimatorController();
                playerManager.isWeaponSwitching = true;
                UnloadArrowOnSlot();
            }
        }
    }
    private void WeaponSwitchTimerSetup() 
    {
        playerManager.isWeaponSwitching = false;
        playerManager.WeaponSwitchTimerSetUp(2f);
    }
    public void WeaponSwitchAnimatorController(bool replaceCurWeapon = false) 
    {
        if (!replaceCurWeapon) //正常切换
        {
            if (!playerManager.isGettingDamage)
            {
                playerAttacker.isUsingPowerArrow = false;
                PlayerInventory playerInventory = playerManager.GetComponent<PlayerInventory>();
                if (mainWeapon_Unequipped == unequippedWeaponSlots[0]) //切换至副武器
                {
                    playerInventory.currentWeaponIndex = 1;
                    mainWeapon_Unequipped = unequippedWeaponSlots[1];
                    transform.GetComponent<Animator>().runtimeAnimatorController = playerManager.GetComponent<PlayerInventory>().unequippedWeaponItems[1].weaponAnimatorController;
                    playerInventory.curEquippedWeaponItem = playerInventory.unequippedWeaponItems[1];
                    mainArmedWeapon = armedWeaponSlot[playerInventory.curEquippedWeaponItem.Id];
                    LoadWeaponOnSlot(playerInventory.unequippedWeaponItems[0], 0);
                    transform.GetComponent<AnimatorManager>().PlayTargetAnimation("WeaponSwitch(Equip)", true, true);
                    if (playerInventory.curEquippedWeaponItem.Id == 2 && playerInventory.powerArrowNum > 0) 
                    {
                        playerAttacker.isUsingPowerArrow = true;
                    }
                }
                else //切换至主武器
                {
                    playerInventory.currentWeaponIndex = 0;
                    mainWeapon_Unequipped = unequippedWeaponSlots[0];
                    transform.GetComponent<Animator>().runtimeAnimatorController = playerManager.GetComponent<PlayerInventory>().unequippedWeaponItems[0].weaponAnimatorController;
                    transform.GetComponent<AnimatorManager>().PlayTargetAnimation("WeaponSwitch(Equip)", true, true);
                    playerInventory.curEquippedWeaponItem = playerInventory.unequippedWeaponItems[0];
                    mainArmedWeapon = armedWeaponSlot[playerInventory.curEquippedWeaponItem.Id];
                    LoadWeaponOnSlot(playerInventory.unequippedWeaponItems[1], 1);
                }
            }
            UnloadArrowOnSlot();
        }
        else //切换当前所装备的武器
        {
            playerAttacker.isUsingPowerArrow = false;
            PlayerInventory playerInventory = playerManager.GetComponent<PlayerInventory>();
            int curIndex = playerInventory.currentWeaponIndex;
            transform.GetComponent<Animator>().runtimeAnimatorController = playerManager.GetComponent<PlayerInventory>().unequippedWeaponItems[curIndex].weaponAnimatorController;
            playerInventory.curEquippedWeaponItem = playerInventory.unequippedWeaponItems[curIndex];
            mainArmedWeapon = armedWeaponSlot[playerInventory.curEquippedWeaponItem.Id];
            LoadWeaponOnSlot(playerInventory.unequippedWeaponItems[curIndex], curIndex);
            transform.GetComponent<AnimatorManager>().PlayTargetAnimation("WeaponSwitch(Equip)", true, true);
            UnloadArrowOnSlot();
        }
    }
    public void AttackSwitchAnimatorController() //切换攻击
    {
        playerManager.transAttackTimer = 0;
        playerManager.canTransAttack = false;
        playerManager.isWeaponSwitching = false;
        playerAttacker.isUsingPowerArrow = false;
        if (mainWeapon_Unequipped == unequippedWeaponSlots[0])
        {
            playerManager.GetComponent<PlayerInventory>().currentWeaponIndex = 1;
            mainWeapon_Unequipped = unequippedWeaponSlots[1];
            mainArmedWeapon = armedWeaponSlot[playerManager.GetComponent<PlayerInventory>().unequippedWeaponItems[1].Id];
            transform.GetComponent<Animator>().runtimeAnimatorController = playerManager.GetComponent<PlayerInventory>().unequippedWeaponItems[1].weaponAnimatorController;
            weaponDamageCollider = mainArmedWeapon.GetComponentInChildren<DamageCollider>();
            if (playerManager.GetComponent<PlayerInventory>().unequippedWeaponItems[1].Id == 2) playerAttacker.isUsingPowerArrow = true;
            playerAttacker.HandleTransformAttack(playerInventory.unequippedWeaponItems[playerInventory.currentWeaponIndex]);
        }
        else
        {
            playerManager.GetComponent<PlayerInventory>().currentWeaponIndex = 0;
            mainWeapon_Unequipped = unequippedWeaponSlots[0];
            mainArmedWeapon = armedWeaponSlot[playerManager.GetComponent<PlayerInventory>().unequippedWeaponItems[0].Id];
            transform.GetComponent<Animator>().runtimeAnimatorController = playerManager.GetComponent<PlayerInventory>().unequippedWeaponItems[0].weaponAnimatorController;
            weaponDamageCollider = mainArmedWeapon.GetComponentInChildren<DamageCollider>();
            if (playerManager.GetComponent<PlayerInventory>().unequippedWeaponItems[0].Id == 2) playerAttacker.isUsingPowerArrow = true;
            playerAttacker.HandleTransformAttack(playerInventory.unequippedWeaponItems[playerInventory.currentWeaponIndex]);
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
    private void RangeShootingStaminaCost(int staminaCost = 20) 
    {
        playerManager.GetComponent<PlayerStats>().CostStamina(staminaCost);
    }
    private void AttackOver() //确定何时提前关闭玩家当前的攻击状态
    {
        animator.SetBool("cantBeInterrupted", false);
        playerManager.isImmuAttack = false;
        inputManager.reAttack_Input = false;
    }
    private void ImmuOver() 
    {
        playerManager.isImmuAttack = false;
    }
    #endregion
}
