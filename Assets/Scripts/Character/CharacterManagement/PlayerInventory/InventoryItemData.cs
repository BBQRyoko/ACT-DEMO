using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemData
{
    /// <summary>
    /// 源数据
    /// </summary>
    public Item Source { get; private set; }
    /// <summary>
    /// 数量
    /// </summary>
    public int Count { get; private set; }
    public Sprite Icon { get { return Source.itemIcon; } }
    public string Name { get { return Source.itemName; } }
    public bool HasHeapUp { get { return Source.HasHeapUp; } }
    public string Des { get { return Source.GetDescript(); } }
    public int ExecutableOperation { get { return Source.ExecutableOperation; } }

    public InventoryItemData(Item source, int count)
    {
        Source = source;
        Count = count;
    }

    public void AddCount(int delta)
    {
        Count += delta;
    }

    public void ReduceCount(int delta)
    {
        Count -= delta;
    }
}
