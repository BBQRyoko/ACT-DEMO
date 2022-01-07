using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInAreaDetection : MonoBehaviour
{
    public CameraManager cameraManager;
    public bool lockOn_Input;
    public bool lockOn_Flag;

    // Start is called before the first frame update
    void Awake()
    {
        cameraManager = FindObjectOfType<CameraManager>();
    }

    private void Update()
    {
        HandleLockOnInput();
    }
    private void HandleLockOnInput() //手动锁定敌人
    {
        if (lockOn_Input && !lockOn_Flag)
        {
            cameraManager.ClearLockOnTargets();
            lockOn_Input = false;
            cameraManager.HandleLockOn();
            if (cameraManager.nearestLockOnTarget != null)
            {
                cameraManager.currentLockOnTarget = cameraManager.nearestLockOnTarget;
                lockOn_Flag = true;
            }
        }
        //else if (lockOn_Input && lockOn_Flag)
        //{
        //    lockOn_Input = false;
        //    lockOn_Flag = false;
        //    //取消锁定
        //    cameraManager.ClearLockOnTargets();
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy")) 
        {
            lockOn_Input = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy")) 
        {
            //cameraManager.ClearLockOnTargets();
            lockOn_Input = false;
            lockOn_Flag = false;
        }
    }
}
