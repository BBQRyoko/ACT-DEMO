using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    WeaponSlotManager WeaponSlotManager;

    public WeaponItem[] unequippedWeaponItems = new WeaponItem[2];

    public List<InventoryItemData> items;

    private void Awake()
    {
        WeaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
        items = new List<InventoryItemData>();
    }

    private void Start()
    {
        WeaponSlotManager.LoadWeaponOnSlot(unequippedWeaponItems[0],0);
        WeaponSlotManager.LoadWeaponOnSlot(unequippedWeaponItems[1],1);
    }
    /// <summary>
    /// 增加道具
    /// </summary>
    /// <param name="item"></param>
    /// <param name="count"></param>
    public void AddItem(Item item, int count)
    {
        if (item.HasHeapUp)
        {
            //可堆叠，检查是否已经存在物品
            foreach (var data in items)
            {
                if (data.Source == item)
                {
                    data.AddCount(count);
                    return;
                }
            }
        }
        //可堆叠且items中不存在该物品或者不可堆叠的情况，新建一个Data容器包装item
        InventoryItemData tData = new InventoryItemData(item, count);
        items.Add(tData);
    }

    /// <summary>
    /// 减少物品
    /// </summary>
    /// <param name="item"></param>
    /// <param name="count"></param>
    public void ReduceItem(Item item, int count)
    {
        InventoryItemData removeItem = null;
        foreach (var data in items)
        {
            if (data.Source == item)
            {
                data.ReduceCount(count);
                if (data.Count <= 0)
                {
                    removeItem = data;
                    break;
                }
            }
        }
        if (removeItem != null)
        {
            items.Remove(removeItem);
        }
    }
}
