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
    InputManager inputManager;

    public bool isOwned;

    public bool canBeSelected;
    bool isSelected;
    [SerializeField] int bagua_Index;
    [SerializeField] int dir_Index;

    RectTransform rect;
    [SerializeField] GameObject unownedBagua;
    [SerializeField] GameObject unselectedBagua;
    [SerializeField] GameObject selectedBagua;

    private void Start()
    {
        baGuaPanel_UI = FindObjectOfType<BaGuaPanel_UI>();
        baGuaManager = FindObjectOfType<BaGuaManager>();
        inputManager = FindObjectOfType<InputManager>();
        rect = GetComponent<RectTransform>();
    }
    private void Update()
    {
        if (!isOwned)
        {
            unownedBagua.SetActive(true);
            unselectedBagua.SetActive(false);
            selectedBagua.SetActive(false);
        }
        else
        {
            if (!isSelected)
            {
                unownedBagua.SetActive(false);
                unselectedBagua.SetActive(true);
                selectedBagua.SetActive(false);
            }
        }
        ControlerPointerCheck();
    }
    void BaGuaEntrySelected() 
    {
        unownedBagua.SetActive(false);
        unselectedBagua.SetActive(false);
        selectedBagua.SetActive(true);
    }

    void ControlerPointerCheck() 
    {
        if (!isSelected && canBeSelected && isOwned && inputManager.baGua_Input) 
        {
            if (Vector2.Distance(transform.position, baGuaManager.realPiviot.transform.position) <= 50f)
            {
                rect.DOComplete();
                rect.DOScale(Vector3.one * 1.2f, 0.1f).SetEase(Ease.OutQuad);
                if (baGuaManager.baguasHolder.Count < baGuaManager.commandSlotNum)
                {
                    baGuaManager.AddBaguaCommand(bagua_Index);
                    isSelected = true;
                    baGuaPanel_UI.CommandFilling(bagua_Index, dir_Index);
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
                    rect.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutQuad);
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
        if (!isSelected && canBeSelected && isOwned && inputManager.baGua_Input) 
        {
            rect.DOComplete();
            rect.DOScale(Vector3.one * 1.2f, 0.1f).SetEase(Ease.OutQuad);
            if (baGuaManager.baguasHolder.Count < baGuaManager.commandSlotNum) 
            {
                baGuaManager.AddBaguaCommand(bagua_Index);
                isSelected = true;
                baGuaPanel_UI.CommandFilling(bagua_Index, dir_Index);
            }
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isOwned) return;

        if (isSelected)
        {
            rect.DOComplete();
            rect.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutQuad);
            BaGuaEntrySelected();
        }
        else 
        {
            rect.DOComplete();
            rect.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutQuad);
        }
    }
}
