using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class BaGuaEntry : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] BaGuaManager baGuaManager;
    BaGuaPanel_UI baGuaPanel_UI;

    public bool isOwned;

    public bool canBeSelected;
    bool isSelected;
    [SerializeField] int bagua_Index;

    RectTransform rect;
    [SerializeField] GameObject unselectedBagua;
    [SerializeField] GameObject selectedBagua;

    private void Start()
    {
        baGuaPanel_UI = FindObjectOfType<BaGuaPanel_UI>();
        baGuaManager = FindObjectOfType<BaGuaManager>();
        rect = GetComponent<RectTransform>();
    }
    private void Update()
    {
        if (!isOwned)
        {
            unselectedBagua.SetActive(false);
            selectedBagua.SetActive(true);
        }
        else
        {
            if (!isSelected)
            {
                unselectedBagua.SetActive(true);
                selectedBagua.SetActive(false);
            }
        }

        ControlerPointerCheck();
    }
    void BaGuaEntrySelected() 
    {
        unselectedBagua.SetActive(false);
        selectedBagua.SetActive(true);
    }

    void ControlerPointerCheck() 
    {
        if (!isSelected && canBeSelected && isOwned) 
        {
            if (Vector2.Distance(transform.position, baGuaManager.realPiviot.transform.position) <= 50f)
            {
                rect.DOComplete();
                rect.DOScale(Vector3.one * 1.2f, 0.1f).SetEase(Ease.OutQuad);
                if (baGuaManager.baguasHolder.Count < baGuaManager.commandSlotNum)
                {
                    baGuaManager.AddBaguaCommand(bagua_Index);
                    isSelected = true;
                    baGuaPanel_UI.CommandFilling(bagua_Index);
                    for (int i = 0; i < baGuaPanel_UI.commandSlotList.Count; i++)
                    {
                        if (!baGuaPanel_UI.commandSlotList[i].commandSelected)
                        {
                            Color32 color = unselectedBagua.GetComponent<RawImage>().color;
                            baGuaPanel_UI.commandSlotList[i].CommandFilling(color);
                            return;
                        }
                    }
                }
            }
        }
        if (rect.localScale.x == 1.2f) //被选上的状态
        {
            if (Vector2.Distance(transform.position, baGuaManager.realPiviot.transform.position) > 50f)
            {
                if (isSelected)
                {
                    rect.DOComplete();
                    rect.DOScale(Vector3.one * 0.85f, 0.1f).SetEase(Ease.OutQuad);
                    BaGuaEntrySelected();
                }
                else
                {
                    rect.DOComplete();
                    rect.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutQuad);
                }
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isSelected && canBeSelected && isOwned) 
        {
            rect.DOComplete();
            rect.DOScale(Vector3.one * 1.2f, 0.1f).SetEase(Ease.OutQuad);
            if (baGuaManager.baguasHolder.Count < baGuaManager.commandSlotNum) 
            {
                baGuaManager.AddBaguaCommand(bagua_Index);
                isSelected = true;
                baGuaPanel_UI.CommandFilling(bagua_Index);
                for (int i = 0; i < baGuaPanel_UI.commandSlotList.Count; i++)
                {
                    if (!baGuaPanel_UI.commandSlotList[i].commandSelected)
                    {
                        Color32 color = unselectedBagua.GetComponent<RawImage>().color;
                        baGuaPanel_UI.commandSlotList[i].CommandFilling(color);
                        return;
                    }
                }
            }
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isOwned) return;

        if (isSelected)
        {
            rect.DOComplete();
            rect.DOScale(Vector3.one * 0.85f, 0.1f).SetEase(Ease.OutQuad);
            BaGuaEntrySelected();
        }
        else 
        {
            rect.DOComplete();
            rect.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutQuad);
        }
    }
}
