using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : InteractSystem
{
    BaGuaManager baGuaManager;

    [Header("Ability")]
    [SerializeField] bool hasAbility;
    public enum Ability {Heal, FireBall, Immu};
    public Ability UnlockAbility;

    private void Start()
    {
        baGuaManager = FindObjectOfType<BaGuaManager>();
    }

    public override void Interact()
    {
        base.Interact();
        if (hasAbility)
        {
            Unlock();
        }
        ProgressSave();
    }

    void Unlock()
    {
        if (UnlockAbility == Ability.Heal)
        {
            baGuaManager.healUnlock = true;
        }
        else if (UnlockAbility == Ability.FireBall)
        {
            baGuaManager.fireBallUnlock = true;
        }
        else if (UnlockAbility == Ability.Immu)
        {
            baGuaManager.immuUnlock = true;
        }
        hasAbility = false;
    }

    void ProgressSave() 
    {
        //Later
    }
}
