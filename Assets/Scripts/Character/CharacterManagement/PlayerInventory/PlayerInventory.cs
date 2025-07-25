using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    PlayerManager playerManager;
    PlayerInventory playerInventory;
    WeaponSlotManager WeaponSlotManager;

    public int currentWeaponIndex;
    public WeaponItem curEquippedWeaponItem;
    public WeaponItem[] unequippedWeaponItems = new WeaponItem[2];
    public WeaponItem katanaWeaponItem;

    public List<InventoryItemData> items;
    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        playerInventory = GetComponent<PlayerInventory>();
        WeaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
        items = new List<InventoryItemData>();
        curEquippedWeaponItem = unequippedWeaponItems[0];
    }

    private void Start()
    {
        WeaponSlotManager.LoadWeaponOnSlot(unequippedWeaponItems[0],0);
    }
    private void Update()
    {
        curEquippedWeaponItem = unequippedWeaponItems[currentWeaponIndex];
        //if (currentWeaponIndex == 0)
        //{
        //    curEquippedWeaponItem = unequippedWeaponItems[1];
        //}
        //else if (currentWeaponIndex == 1)
        //{
        //    curEquippedWeaponItem = unequippedWeaponItems[0];
        //} 
    }
    public void PickUpWeapon(WeaponItem weapon) 
    {
        if (playerInventory.unequippedWeaponItems[1] == null)
        {
            playerInventory.unequippedWeaponItems[1] = weapon;
            playerInventory.curEquippedWeaponItem = unequippedWeaponItems[0];
            WeaponPickingEquip();
        }
        else
        {
            if (currentWeaponIndex == 0)
            {
                playerInventory.unequippedWeaponItems[0] = weapon;
                playerInventory.curEquippedWeaponItem = unequippedWeaponItems[1];
                WeaponPickingEquip(true);
            }
            else if (currentWeaponIndex == 1) 
            {
                playerInventory.unequippedWeaponItems[1] = weapon;
                playerInventory.curEquippedWeaponItem = unequippedWeaponItems[0];
                WeaponPickingEquip(true);
            }
        }
    }

    public void WeaponPickingEquip(bool replaceCurWeapon = false) 
    {
        playerManager.GetComponentInChildren<WeaponSlotManager>().mainArmedWeapon.SetActive(false);
        if (!replaceCurWeapon)
        {
            playerManager.GetComponentInChildren<WeaponSlotManager>().mainWeapon_Unequipped.gameObject.SetActive(true);
        }
        playerManager.GetComponentInChildren<WeaponSlotManager>().WeaponSwitchAnimatorController(replaceCurWeapon);
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
