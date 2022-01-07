using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UnityUtil
{
    /// <summary>
    /// 自动将source锚定在Target旁边
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    public static void AnchorTargetByScreen(RectTransform source, RectTransform target)
    {
        //判断方向
        float ax = Mathf.Clamp01(target.position.x / Screen.width);
        float ay = Mathf.Clamp01(target.position.y / Screen.height);
        float px = ax >= 0.5f ? 1 : 0;
        float py = ay >= 0.5f ? 1 : 0;
        source.anchorMin = new Vector2(ax, ay);
        source.anchorMax = new Vector2(ax, ay);
        source.pivot = new Vector2(px, py);
        float x = target.rect.width * target.pivot.x * (px == 1 ? -1 : 1);
        float y = target.rect.height * target.pivot.y * (py == 1 ? 1 : -1);
        Debug.Log($"width = {target.rect.width}, height = {target.rect.height}");
        source.anchoredPosition = new Vector2(x, y);
    }
}
