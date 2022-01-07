using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : CharacterManager
{
    //PlayerManager统一管理所有当前所处的状态, 与locomotion, input, camera的update
    Animator animator;
    InputManager inputManager;
    CameraManager cameraManager;
    PlayerLocmotion playerLocmotion;
    PlayerStats playerStats;
    AnimatorManager animatorManager;
    WeaponSlotManager weaponSlotManager;
    Rigidbody rig;

    [Header("运动状态")]
    public bool isInteracting;
    public bool isUsingRootMotion;

    public bool isFalling; //下落时
    public bool isGround; //在地面时
    public bool isSprinting; 
    public bool isRolling;
    public bool isJumping; //跳跃上升阶段

    //战斗
    public bool isWeaponEquipped;
    public bool isHitting;
    public bool isAttacking;
    public bool cantBeInterrupted;
    public bool hitRecover;
    public bool isStunned;
    public bool damageAvoid;

    //武器切换相关
    public float perfectTimer;
    public bool isPerfect;

    //蓄力攻击相关
    public bool isCharging;
    public bool isHolding;
    public bool isAttackDashing;

    //完美格挡ATField
    [SerializeField] GameObject aT_Field_Prefab;
    [SerializeField] Transform aT_position;

    private void Awake()
    {
        cameraManager = FindObjectOfType<CameraManager>();
        animator = GetComponentInChildren<Animator>();
        inputManager = GetComponent<InputManager>();
        playerLocmotion = GetComponent<PlayerLocmotion>();
        playerStats = GetComponent<PlayerStats>();
        animatorManager = GetComponentInChildren<AnimatorManager>();
        weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
        rig = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        inputManager.HandleAllInputs();
        playerStats.StaminaRegen();
        CheckForInteractableObject();
        PerfectTimer();
    }
    private void FixedUpdate()
    {
        playerLocmotion.HandleAllMovement();
        cameraManager.HandleAllCameraMovement();
    }
    private void LateUpdate()
    {
        isInteracting = animator.GetBool("isInteracting");
        isAttacking = animator.GetBool("isAttacking");
        isUsingRootMotion = animator.GetBool("isUsingRootMotion");
        isCharging = animator.GetBool("isCharging");
        isHolding = animator.GetBool("isHolding");
        animator.SetBool("cantBeInterrupted", cantBeInterrupted);
        animator.SetBool("isStunned", isStunned);
        animator.SetBool("isGround", isGround); 
        animator.SetBool("isFalling", isFalling);
        inputManager.reAttack_Input = false;
        inputManager.interact_Input = false;
        inputManager.weaponSwitch_Input = false;
        HoldingAction();
        ChargingAction();
    }
    private void CheckForInteractableObject() 
    {
        RaycastHit hit;

        if (Physics.SphereCast(transform.position, 0.4f, transform.forward, out hit, 0.8f, cameraManager.ignoreLayers, QueryTriggerInteraction.Collide)) 
        {
            if (hit.collider.tag == "Interactable") 
            {
                Interactable interactableObject = hit.collider.gameObject.GetComponent<Interactable>();

                if (interactableObject != null) 
                {
                    string interactableText = interactableObject.interactableText;

                    if (inputManager.interact_Input && !isWeaponEquipped) 
                    {
                        hit.collider.GetComponent<Interactable>().Interact(this);
                    }
                }
            }
        }
    }
    public void GetDebuff(float duration) //当前只有stun
    {
        animatorManager.PlayTargetAnimation("StunTest", true);
        isStunned = true;
        rig.velocity = Vector3.zero;
        StartCoroutine(stunTimer(duration));
    }
    private void ChargingAction() //攻击蓄力
    {
        if (!isCharging)
        {
            inputManager.spAttack_Input = false;
        }
        else
        {
            inputManager.spAttack_Input = true;
        }
    }
    private void HoldingAction() //按键保持
    {
        if (!isHolding)
        {
            inputManager.weaponAbility_Input = false;
        }
        else 
        {
            inputManager.weaponAbility_Input = true;
        }
    }
    public void weaponEquiping(bool beDamaging = false) 
    {
        if (!beDamaging)
        {
            if (!isInteracting)
            {
                if (!isWeaponEquipped)
                {
                    animatorManager.PlayTargetAnimation("Equip", true, true);
                    isWeaponEquipped = true;
                }
                else
                {
                    animatorManager.PlayTargetAnimation("Unarm", true, true);
                    isWeaponEquipped = false;
                }
            }
        }
        else
        {
            if (!isWeaponEquipped)
            {
                isWeaponEquipped = true;
                weaponSlotManager.EquipeWeapon();
            }
        }
    }
    public void PerfectTimer() 
    {
        if (perfectTimer>0) 
        {
            isPerfect = true;
            perfectTimer -= Time.deltaTime;
            if (perfectTimer <= 0) 
            {
                perfectTimer = 0;
                isPerfect = false;
            }
        }
    }
    public void PerfectBlock() 
    {
        Debug.Log("AT FILED!!!!");
        animatorManager.PlayTargetAnimation("WeaponAbility_01(Success)", true, true);
        GameObject AT_Field_Temp = Instantiate(aT_Field_Prefab, aT_position.position, Quaternion.identity);
        AT_Field_Temp.transform.SetParent(null);
    }

    IEnumerator stunTimer(float dur) //播放器暂停
    {
        yield return new WaitForSecondsRealtime(dur);
        isStunned = false;
    }
}
