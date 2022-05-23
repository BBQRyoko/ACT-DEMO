using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BaGuaPanel_UI : MonoBehaviour
{
    InputManager inputManager;
    BaGuaManager baGuaManager;
    [SerializeField]List<BaGuaEntry> baguaList;

    [SerializeField] float panelRaduis;
    public bool actived;

    private void Start()
    {
        baGuaManager = FindObjectOfType<BaGuaManager>();
        inputManager = FindObjectOfType<InputManager>();
        baguaList = new List<BaGuaEntry>();
    }
    private void Update()
    {
        PanelSetActiveController();
    }
    void PanelActive()
    {
        for (int i = 0; i < baGuaManager.baguaGameobjectPrefabs.Length; i++) 
        {
            AddBaguaEntry(baGuaManager.baguaGameobjectPrefabs[i]);
        }
        BaGuaRearrange();
        actived = true;
    }
    void PanelClose() 
    {
        for (int i = 0; i < baGuaManager.baguaGameobjectPrefabs.Length; i++)
        {
            RectTransform rect = baguaList[i].GetComponent<RectTransform>();
            GameObject entry = baguaList[i].gameObject;

            rect.DOScale(Vector3.zero, 0.15f).SetEase(Ease.OutQuad);
            rect.DOAnchorPos(Vector3.zero, .15f).SetEase(Ease.OutQuad).onComplete =
                delegate ()
                {
                    Destroy(entry);
                    gameObject.SetActive(false);
                };
        }
        actived = false;
        baguaList.Clear();
        baGuaManager.BaguaCasting();
    }
    void PanelSetActiveController() 
    {
        if (inputManager.baGua_Input && !actived)
        {
            PanelActive();
        }
        else if (!inputManager.baGua_Input && actived)
        {
            PanelClose();
        }
        else if (!inputManager.baGua_Input)
        {
            gameObject.SetActive(false);
        }
    }
    public void AddBaguaEntry(GameObject prefab) 
    {
        GameObject entry = Instantiate(prefab, transform);
        BaGuaEntry baGuaEntry = entry.GetComponent<BaGuaEntry>();
        baguaList.Add(baGuaEntry);
    }
    void BaGuaRearrange() 
    {
        float radiansOfSeparation = (Mathf.PI * 2) / baguaList.Count;
        for (int i = 0; i < baguaList.Count; i++) 
        {
            float x = Mathf.Sin(radiansOfSeparation * i) * panelRaduis;
            float y = Mathf.Cos(radiansOfSeparation * i) * panelRaduis;

            RectTransform rect = baguaList[i].GetComponent<RectTransform>();

            
            rect.localScale = Vector3.zero;
            rect.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutQuad);
            rect.DOAnchorPos(new Vector3(x, y, 0f), 0.25f).SetEase(Ease.OutQuad).onComplete =
                delegate ()
                {
                    BaGuaEntry baGuaEntry = rect.GetComponent<BaGuaEntry>();
                    baGuaEntry.canBeSelected = true;
                };
        }
    }
}
