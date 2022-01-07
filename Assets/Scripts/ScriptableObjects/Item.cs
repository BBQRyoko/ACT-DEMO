using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/New Regular Item")]
public class Item : ScriptableObject
{
    /// <summary>
    /// 编号，唯一不重复
    /// </summary>
    public int Id;
    [Header("道具信息")]
    public Sprite itemIcon;
    public string itemName;
    public bool HasHeapUp;
    /// <summary>
    /// 可进行的操作：0 全部 1 没有使用
    /// </summary>
    public int ExecutableOperation;
    public string GetDescript()
    {
        return $"道具---->{itemName}";
    }
}
