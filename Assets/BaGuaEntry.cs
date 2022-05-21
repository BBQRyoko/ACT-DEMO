using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class BaGuaEntry : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    BaGuaManager baGuaManager;
    BaGuaPanel_UI BaGuaPanel_UI;
    public bool isSelected;
    public int bagua_Index;

    [SerializeField] RectTransform rect;
    [SerializeField] GameObject unselectedBagua;
    [SerializeField] GameObject selectedBagua;

    private void Start()
    {
        //rect = GetComponent<RectTransform>();
    }
    private void Update()
    {
        if(!isSelected)
        {
            unselectedBagua.SetActive(true);
            selectedBagua.SetActive(false);
        }
    }
    //被选定时
    void BaGuaEntrySelected() 
    {
        unselectedBagua.SetActive(false);
        selectedBagua.SetActive(true);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isSelected) 
        {
            rect.DOComplete();
            rect.DOScale(Vector3.one * 0.2f, 0.1f).SetEase(Ease.OutQuad);
            isSelected = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isSelected)
        {
            rect.DOComplete();
            rect.DOScale(Vector3.one * 0.15f, 0.1f).SetEase(Ease.OutQuad);
            BaGuaEntrySelected();
        }
    }
}
