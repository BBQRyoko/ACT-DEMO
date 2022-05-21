using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BaGuaPanel_UI : MonoBehaviour
{
    InputManager inputManager;
    public List<BaGuaEntry> BaguaList = new List<BaGuaEntry>();
    [SerializeField] float panelRaduis;

    public bool actived;

    private void Start()
    {
        inputManager = FindObjectOfType<InputManager>();
    }
    private void Update()
    {
        if (inputManager.baGua_Input && !actived) 
        {
            PanelActive();
        }
    }

    void PanelActive() 
    {
        for (int i = 0; i < BaguaList.Count; i++) 
        {
            BaGuaEntry baguaEntry = Instantiate(BaguaList[i], transform);
        }
        BaGuaRearrange();
        actived = true;
    }

    public void AddBaguaEntry() 
    {
    
    }

    void BaGuaRearrange() 
    {
        float radiansOfSeparation = (Mathf.PI * 2) / BaguaList.Count;
        for (int i = 0; i < BaguaList.Count; i++) 
        {
            float x = Mathf.Sin(radiansOfSeparation * i) * panelRaduis;
            float y = Mathf.Cos(radiansOfSeparation * i) * panelRaduis;

            RectTransform rect = BaguaList[i].GetComponent<RectTransform>();

            rect.localScale = Vector3.zero;
            rect.DOScale(Vector3.one, 2f).SetEase(Ease.OutQuad).SetDelay(0.05f * i);
            rect.GetComponent<RectTransform>().DOAnchorPos(new Vector3(x, y, 0), 2f).SetEase(Ease.OutQuad).SetDelay(0.05f * i);
        }
    }
}
