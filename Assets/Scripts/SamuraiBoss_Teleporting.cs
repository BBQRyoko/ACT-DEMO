using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SamuraiBoss_Teleporting : MonoBehaviour
{
    [SerializeField] InputManager inputManager;
    [SerializeField] CameraManager cameraManager;
    EnemyManager enemyManager;
    [SerializeField] Transform teleportingPos;

    public Material dissolveMatrial;
    public float dissolveValue;
    public bool dissolving;
    bool teleported;

    [SerializeField] bool isFinalBoss;

    [SerializeField] GameObject leafWindVFX;
    [SerializeField] CombatStanceState CombatStanceState;
    // Start is called before the first frame update
    void Start()
    {
        inputManager = FindObjectOfType<InputManager>();
        cameraManager = FindObjectOfType<CameraManager>();
        enemyManager = GetComponent<EnemyManager>();
        dissolveMatrial.SetFloat("Dissolve", 1); //Default Vaule
    }

    void Update()
    {
        dissolveMatrial.SetFloat("Dissolve", dissolveValue);

        if (CombatStanceState.distanceFromTarget <= 10 && CombatStanceState.distanceFromTarget > 0 && !isFinalBoss) 
        {
            TeleportStartEvent();
        }
        if (isFinalBoss && enemyManager.GetComponent<EnemyStats>().currHealth <= 600  && !enemyManager.phaseChanged && !enemyManager.isInteracting) 
        {
            TeleportStartEvent();
            enemyManager.phaseChanged = true;
            //给enemyManager加一个特殊条件，在结束前保持半血且无敌
        }

        if (dissolveValue <= 0)
        {
            enemyManager.attackLock = false;
        }
        else
        {
            enemyManager.attackLock = true;
        }

        if (dissolving)
        {
            if (dissolveValue < 0.85f)
            {
                dissolveValue += 0.45f * Time.deltaTime;
            }
            else 
            {
                leafWindVFX.SetActive(false);
                TeleportingEvent();
            }
        }
        else 
        {
            if (dissolveValue > 0)
            {
                dissolveValue -= 0.65f * Time.deltaTime;
                if (dissolveValue <= 0) 
                {
                    if (enemyManager.phaseChanged && isFinalBoss)
                    {
                        enemyManager.isPhaseChaging = true;
                        enemyManager.GetComponentInChildren<EnemyAnimatorManager>().animator.SetTrigger("teleportingComplete");
                    }
                }
            }
        }
    }

    void TeleportStartEvent()
    {
        leafWindVFX.SetActive(true);
        dissolving = true;
        if (dissolveValue <= 0) 
        {
            enemyManager.GetComponentInChildren<EnemyAnimatorManager>().PlayTargetAnimation("DodgeB(Medium)", true,true);
            enemyManager.GetComponentInChildren<Animator>().SetTrigger("isTeleporting");
        }
    }

    void TeleportingEvent() 
    {
        transform.position = teleportingPos.position;
        dissolving = false;
        if (!isFinalBoss)
        {
            inputManager.lockOn_Input = false;
            inputManager.lockOn_Flag = false;
            cameraManager.ClearLockOnTargets();
            Destroy(gameObject, 8f);
        }
    }
}
