using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockGua : InteractSystem
{
    BaGuaManager baGuaManager;
    AnimatorManager animatorManager;
    [SerializeField] GameObject sealedBagua;
    [SerializeField] int guaIndex;
    [SerializeField] GameObject guide;

    private void Start()
    {
        baGuaManager = FindObjectOfType<BaGuaManager>();
        animatorManager = FindObjectOfType<AnimatorManager>();
    }
    public override void Interact()
    {
        base.Interact();
        UnlockNewGuaEntry();
    }

    void UnlockNewGuaEntry()
    {
        if (!baGuaManager.baguaGameobjectPrefabs[guaIndex].GetComponent<BaGuaEntry>().isOwned)
        {
            baGuaManager.baguaGameobjectPrefabs[guaIndex].GetComponent<BaGuaEntry>().isOwned = true;
            sealedBagua.SetActive(false);
            guide.SetActive(true);
            animatorManager.generalAudio.clip = animatorManager.sample_SFX.checkPoint_Heal[0];
            animatorManager.generalAudio.Play();
        }
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!baGuaManager.baguaGameobjectPrefabs[guaIndex].GetComponent<BaGuaEntry>().isOwned)
            {
                other.GetComponent<PlayerManager>().inInteractTrigger = true;
                HandleInteractUI(this);
            }
            else 
            {
                guide.SetActive(true);
            }
        }
    }

    public override void OnTriggerExit(Collider other) 
    {
        if (other.CompareTag("Player"))
        {
            guide.SetActive(false);
        }
    }
}
