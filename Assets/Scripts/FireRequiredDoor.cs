using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireRequiredDoor : InteractSystem
{
    PlayerManager playerManager;
    public bool doorIsOpened;
    // Start is called before the first frame update
    void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();
    }
    public override void Interact()
    {
        base.Interact();
        BaGuaManager baGuaManager = playerManager.GetComponent<BaGuaManager>();
        AnimatorManager animatorManager = playerManager.GetComponentInChildren<AnimatorManager>();
        //if (baGuaManager.fireBallUnlock)
        //{
        //    //将玩家位置放到指定位置，指定朝向
        //    animatorManager.PlayTargetAnimation("FireBall", true, true);
        //}
        //else
        //{
        //    Debug.Log("FireBall Required");
        //}
    }
}
