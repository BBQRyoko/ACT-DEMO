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
    [SerializeField] GameObject leafWindVFX;
    [SerializeField] CombatStanceState CombatStanceState;
    // Start is called before the first frame update
    void Start()
    {
        inputManager = FindObjectOfType<InputManager>();
        cameraManager = FindObjectOfType<CameraManager>();
        enemyManager = GetComponent<EnemyManager>();
        dissolveMatrial.SetFloat("Dissolve", 0); //Default Vaule
    }

    void Update()
    {
        dissolveMatrial.SetFloat("Dissolve", dissolveValue);

        if (CombatStanceState.distanceFromTarget <= 15 && CombatStanceState.distanceFromTarget >0) 
        {
            TeleportStartEvent();
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
            if (dissolveValue < 1)
            {
                dissolveValue += 0.35f * Time.deltaTime;
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
                dissolveValue -= 0.35f * Time.deltaTime;
            }
        }
    }

    void TeleportStartEvent() 
    {
        leafWindVFX.SetActive(true);
        dissolving = true;
    }

    void TeleportingEvent() 
    {
        enemyManager.AutoLockOn();
        transform.position = teleportingPos.position;
        leafWindVFX.SetActive(true);
        dissolving = false;
    }
}
