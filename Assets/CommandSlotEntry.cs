using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CommandSlotEntry : MonoBehaviour
{
    BaGuaManager baGuaManager;
    [SerializeField] Image slotFillColor;
    RectTransform rect;

    public bool commandSelected;
    void Start()
    {
        slotFillColor = GetComponent<Image>();
        baGuaManager = FindObjectOfType<BaGuaManager>();
    }

    // Update is called once per frame
    //void Update()
    //{
    //    CommandSlotIconController();
    //}
    //void CommandSlotIconController()
    //{
    //    if (!commandSelected)
    //    {
    //        slotFillColor.fillAmount = 0;
    //    }
    //    else
    //    {
    //        slotFillColor.fillAmount += 1.5f * Time.deltaTime;
    //        if (slotFillColor.fillAmount >= (1 / (float)baGuaManager.commandSlotNum))
    //        {
    //            slotFillColor.fillAmount = 1 / (float)baGuaManager.commandSlotNum;
    //        }
    //    }
    //}
    //public void CommandFilling(Color32 color)
    //{
    //    commandSelected = true;
    //    slotFillColor.color = color;
    //}
}
