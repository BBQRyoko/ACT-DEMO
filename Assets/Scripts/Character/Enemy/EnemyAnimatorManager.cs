using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimatorManager : MainAnimatorManager
{
    [SerializeField] bool isDarkKnight;
    CameraManager cameraManager;
    public EnemyManager enemyManager;
    public float animatorSpeed;
    public Collider damageCollider;

    //Rotate Related
    public bool rotatingWithPlayer;

    //SFX
    public AudioSource attackAudio;
    public AudioSource generalAudio;
    public Sample_SFX sample_SFX;

    [Header("DarkKnight Only")]
    [SerializeField] GameObject TornadoArea;
    [SerializeField] GameObject TornadoSlash;
    [SerializeField] float tornadoSlashTimer;
    public bool tornadoSlashEnhance;

    private void Awake()
    {
        cameraManager = FindObjectOfType<CameraManager>();
        animator = GetComponent<Animator>();
        enemyManager = GetComponentInParent<EnemyManager>();
    }
    private void Update()
    {
        animatorSpeed = animator.speed;
        if (rotatingWithPlayer)
        {
            RotateHandler();
        }
        else
        {
            enemyManager.rotationSpeed = 1f;
        }

        //DarkKnightOnly
        if (isDarkKnight) 
        {
            if (tornadoSlashEnhance)
            {
                TornadoSlash.SetActive(true);
                animator.speed = 1.25f;
            }
            else
            {
                TornadoSlash.SetActive(false);
                animator.speed = 1f;
            }
        }
    }
    private void OnAnimatorMove()
    {
        enemyManager.enemyRig.drag = 0;
        Vector3 deltaPosition = animator.deltaPosition;
        deltaPosition.y = 0;
        Vector3 velocity = deltaPosition / Time.deltaTime;
        enemyManager.enemyRig.velocity = velocity;

        if (enemyManager.isRotatingWithRootMotion) 
        {
            enemyManager.transform.rotation *= animator.deltaRotation;
        }    
    }
    private void AnimatorPlaySpeed(float playRate) //控制动画器的播放速度
    {
        animator.speed = playRate;
    }
    private void MovingDuringAnimation(float movingForce)
    {
        Vector3 dir = new Vector3(enemyManager.curTarget.transform.position.x - enemyManager.transform.position.x, enemyManager.curTarget.transform.position.y - enemyManager.transform.position.y, enemyManager.curTarget.transform.position.z - enemyManager.transform.position.z);
        dir.Normalize();
        enemyManager.GetComponent<Rigidbody>().AddForce(dir * movingForce, ForceMode.Impulse);
    }
    public void EnableDamageCollider()
    {
        damageCollider.enabled = true;
    }
    public void DisableDamageCollider()
    {
        damageCollider.enabled = false;
    }
    private void AnimatorPlaySound(int clipNum) //选择播放的音频
    {
        attackAudio.volume = 0.07f;
        attackAudio.clip = sample_SFX.curSFX_List[clipNum];
        attackAudio.Play();
    }
    private void PlayEquipSound() 
    {
        generalAudio.volume = 0.07f;
        generalAudio.clip = sample_SFX.EquipSFX;
        generalAudio.Play();
    }
    private void RotateTowardsTarget() 
    {
        if (rotatingWithPlayer)
        {
            rotatingWithPlayer = false;
        }
        else 
        {
            rotatingWithPlayer = true;
        }
    }
    private void ChangeRotateSpeed(float rotateSpeed) 
    {
        enemyManager.rotationSpeed = rotateSpeed;
    }
    private void RotateHandler() 
    {
        if (enemyManager.curTarget) 
        {
            Vector3 direction = enemyManager.curTarget.transform.position - transform.position;
            direction.y = 0;
            direction.Normalize();

            if (direction == Vector3.zero)
            {
                direction = transform.forward;
            }

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            enemyManager.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, enemyManager.rotationSpeed);
        }
    }
    private void DeadFreeze() 
    {
        enemyManager.enemyRig.isKinematic = true;
    }
    void OpenParryCollider() 
    {
        enemyManager.isParrying = true;
    }
    void CloseParryCollider() 
    {
        enemyManager.isParrying = false;
    }
    private void ResetFirstStrike() 
    {
        if (enemyManager.firstStrikeTimer <= 0) 
        {
            enemyManager.isFirstStrike = true;
            enemyManager.curState = enemyManager.transform.GetComponentInChildren<PursueState>();
        }
    }
    private void DodgingEnd() 
    {
        if (enemyManager.isDodging)
        {
            enemyManager.isDodging = false;
        }
        else 
        {
            enemyManager.isDodging = true;
        }
    }
    void ExecuteDeadCheck() 
    {
        if (enemyManager.isDead) 
        {
            animator.SetTrigger("isDead");
        }
    }
    private void EnemyDangerWarning() 
    {
        cameraManager.DangerWarning(enemyManager);
    }

    //DarkKnight Only
    void CombatGroundInitial() 
    {
        TornadoArea.SetActive(true);
        TornadoArea.transform.parent = null;
    }
    void TornadoSlashBegin(int time) 
    {
        StartCoroutine(Timer(time));
    }
    public void TornadoSlashSpawn() 
    {
        TornadoSlash.SetActive(true);
    }
    IEnumerator Timer(int dur) //播放器暂停
    {
        float pauseTime = dur;
        yield return new WaitForSecondsRealtime(pauseTime);
        animator.SetTrigger("loopStop");
        tornadoSlashEnhance = false;
    }
}
