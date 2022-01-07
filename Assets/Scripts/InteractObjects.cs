using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractObjects : Interactable
{
    public enum interactType {self, others};
    [SerializeField] interactType curInteractType;

    [SerializeField] Item[] requiredItems;
    public int requiredNum;
    public int requireCount;
    [SerializeField] GameObject[] interactingGameObject; //互动后被操作的物件

    public override void Interact(PlayerManager playerManager)
    {
        base.Interact(playerManager);

        IntreactingObjects(playerManager);
    }

    void IntreactingObjects(PlayerManager playerManager) 
    {
        PlayerInventory playerInventory;
        PlayerLocmotion playerLocmotion;
        AnimatorManager animatorManager;
        playerInventory = playerManager.GetComponent<PlayerInventory>();
        playerLocmotion = playerManager.GetComponent<PlayerLocmotion>();
        animatorManager = playerManager.GetComponentInChildren<AnimatorManager>();

        if (curInteractType == interactType.self) 
        {
            foreach (Item required in requiredItems)
            {
                requiredNum = requiredItems.Length;
                foreach (var item in playerInventory.items)
                {
                    if (item.Name== required.itemName)
                    {
                        requireCount += item.Count;
                    }
                }
                if (requireCount == requiredNum)
                {
                    playerLocmotion.rig.velocity = Vector3.zero;
                    animatorManager.PlayTargetAnimation("Interact", true);
                    Destroy(this.gameObject);
                }
                else if(requireCount<= requiredNum)
                {
                    Debug.Log("但你需要一把钥匙");
                }
            }
        }
    }
}
