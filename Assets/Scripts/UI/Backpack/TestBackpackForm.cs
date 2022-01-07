using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBackpackForm : MonoBehaviour
{
    [Header("测试用的")]
    public PlayerInventory Inventory;
    public Item AddItem;
    public int Count;
    public Item AddItem1;
    public int Count1;
    public Item AddItem2;
    public int Count2;

    // Start is called before the first frame update
    void Start()
    {
        Inventory.AddItem(AddItem, Count);
        Inventory.AddItem(AddItem1, Count1);
        Inventory.AddItem(AddItem2, Count2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
