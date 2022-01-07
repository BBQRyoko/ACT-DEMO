using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParryCollider : MonoBehaviour
{
    PlayerManager playerManager;
    Collider blockCollider;
    public bool isPerfect;
    [SerializeField] float perfectTimer=0.4f;
    [SerializeField] float countDownTimer;

    private void Awake()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        blockCollider = GetComponent<Collider>();
        blockCollider.gameObject.SetActive(true);
        blockCollider.isTrigger = true;
        blockCollider.enabled = false;
    }

    private void FixedUpdate()
    {
        if (countDownTimer > 0)
        {
            isPerfect = true;
            countDownTimer -= Time.deltaTime;
        }
        else 
        {
            countDownTimer = 0;
            isPerfect = false;
        }
    }
    public void EnableParryCollider() 
    {
        blockCollider.enabled = true;
    }

    public void DisableParryCollider() 
    {
        blockCollider.enabled = false;
    }

    public void PerfectTiming() 
    {
        countDownTimer = perfectTimer;
    }
}
