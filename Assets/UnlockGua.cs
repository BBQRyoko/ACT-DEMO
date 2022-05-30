using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockGua : InteractSystem
{
    BaGuaManager baGuaManager;
    [SerializeField] GameObject baguaPrefab;
    [SerializeField] GameObject sealedBagua;

    private void Start()
    {
        baGuaManager = FindObjectOfType<BaGuaManager>();
    }
    public override void Interact()
    {
        base.Interact();
        UnlockNewGuaEntry();
    }

    void UnlockNewGuaEntry()
    {
        if (!baGuaManager.baguaGameobjectPrefabs.Contains(baguaPrefab))
        {
            baGuaManager.baguaGameobjectPrefabs.Add(baguaPrefab);
            sealedBagua.SetActive(false);
        }
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!baGuaManager.baguaGameobjectPrefabs.Contains(baguaPrefab))
            {
                other.GetComponent<PlayerManager>().inInteractTrigger = true;
                HandleInteractUI(this);
            }
        }
    }
}
