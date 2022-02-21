using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/New Regular Item")]
public class Item : ScriptableObject
{
    public int Id;
    [Header("道具信息")]
    public Sprite itemIcon;
    public string itemName;
    public bool HasHeapUp;

    public int ExecutableOperation;
    public string GetDescript()
    {
        return $"道具---->{itemName}";
    }
}
