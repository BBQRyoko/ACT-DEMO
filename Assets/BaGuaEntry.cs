﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class BaGuaEntry : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] BaGuaManager baGuaManager;
    BaGuaPanel_UI BaGuaPanel_UI;
    public bool canBeSelected;
    bool isSelected;
    [SerializeField] int bagua_Index;

    RectTransform rect;
    [SerializeField] GameObject unselectedBagua;
    [SerializeField] GameObject selectedBagua;

    private void Start()
    {
        baGuaManager = FindObjectOfType<BaGuaManager>();
        rect = GetComponent<RectTransform>();
    }
    private void Update()
    {
        if(!isSelected)
        {
            unselectedBagua.SetActive(true);
            selectedBagua.SetActive(false);
        }
    }
    void BaGuaEntrySelected() 
    {
        unselectedBagua.SetActive(false);
        selectedBagua.SetActive(true);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isSelected && canBeSelected) 
        {
            rect.DOComplete();
            rect.DOScale(Vector3.one * 1.2f, 0.1f).SetEase(Ease.OutQuad);
            if (baGuaManager.baguasHolder.Count < baGuaManager.commandSlotNum) 
            {
                baGuaManager.AddBaguaCommand(bagua_Index);
                isSelected = true;
            }

        }
    }
    public void OnPointerExit(PointerEventData eventData)
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