using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : InteractSystem
{
    PlayerManager playerManager;
    GameManager gameManager;
    BaGuaManager baGuaManager;
    [SerializeField] Transform respawnPos;

    [Header("Ability")]
    [SerializeField] bool hasAbility;
    public enum Ability {Heal, FireBall, Immu};
    public Ability UnlockAbility;

    private void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        gameManager = FindObjectOfType<GameManager>();
        baGuaManager = FindObjectOfType<BaGuaManager>();
    }
    public override void Interact()
    {
        base.Interact();
        if (hasAbility)
        {
            Unlock();
        }
        else 
        {
            ProgressSave();
        }
    }
    void Unlock()
    {
        hasAbility = false;
    }

    void ProgressSave() 
    {
        gameManager.curCheckPoint = respawnPos;
        playerManager.Rest();
    }
}
