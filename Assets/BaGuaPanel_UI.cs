using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class BaGuaPanel_UI : MonoBehaviour
{
    InputManager inputManager;
    BaGuaManager baGuaManager;
    [SerializeField] GameObject commandSlotPrefab;
    List<BaGuaEntry> baguaList;
    [SerializeField] Transform commandSlotsHolder;
    public List<CommandSlotEntry> commandSlotList;
    [SerializeField] RawImage baguaIcon;
    [SerializeField] TextMeshProUGUI commandName;
    public string commandString;


    [SerializeField] float panelRaduis;
    public bool actived;

    private void Start()
    {
        baGuaManager = FindObjectOfType<BaGuaManager>();
        inputManager = FindObjectOfType<InputManager>();
        baguaList = new List<BaGuaEntry>();
        commandSlotList = new List<CommandSlotEntry>();
    }
    private void Update()
    {
        PanelSetActiveController();
        CommandNameCheck();
    }
    void PanelActive()
    {
        commandString = null;
        for (int i = 0; i < baGuaManager.baguaGameobjectPrefabs.Count; i++) 
        {
            AddBaguaEntry(baGuaManager.baguaGameobjectPrefabs[i]);
        }
        for (int i = 0; i < baGuaManager.commandSlotNum; i++)
        {
            AddCommandSlot();
        }
        BaGuaRearrange();
        CommandSlotRerrange();
        actived = true;
    }
    void PanelClose() 
    {
        for (int i = 0; i < baGuaManager.baguaGameobjectPrefabs.Count; i++)
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
        for (int i = 0; i < baGuaManager.commandSlotNum; i++)
        {
            RectTransform rect = commandSlotList[i].GetComponent<RectTransform>();
            GameObject entry = commandSlotList[i].gameObject;

            rect.DOScale(Vector3.zero, 0.15f).SetEase(Ease.OutQuad);
            rect.DOAnchorPos(Vector3.zero, .15f).SetEase(Ease.OutQuad).onComplete =
                delegate ()
                {
                    Destroy(entry);
                    gameObject.SetActive(false);
                };
        }
        commandName.gameObject.SetActive(false);
        actived = false;
        baguaList.Clear();
        commandSlotList.Clear();
        if (baGuaManager.baguasHolder.Count >= 2)
        {
            baGuaManager.BaguaCasting();
        }
        else 
        {
            baGuaManager.baguasHolder.Clear();
        }

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
    void AddBaguaEntry(GameObject prefab) 
    {
        GameObject entry = Instantiate(prefab, transform);
        BaGuaEntry baGuaEntry = entry.GetComponent<BaGuaEntry>();
        baguaList.Add(baGuaEntry);
    }
    void AddCommandSlot()
    {
        GameObject entry = Instantiate(commandSlotPrefab, commandSlotsHolder);
        CommandSlotEntry slotEntry = entry.GetComponent<CommandSlotEntry>();
        commandSlotList.Add(slotEntry);
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
    void CommandSlotRerrange()
    {
        float radiansOfSeparation = (-360) / commandSlotList.Count;
        for (int i = 0; i < commandSlotList.Count; i++)
        {
            RectTransform rect = commandSlotList[i].GetComponent<RectTransform>();

            rect.DOAnchorPos(Vector3.zero, 0f);
            rect.DORotateQuaternion(Quaternion.Euler(0, 0, radiansOfSeparation * (i+1)), 0f);
        }
    }
    public void CommandFilling(int index)
    {
        baguaIcon.gameObject.SetActive(true);
        baguaIcon.texture = baGuaManager.baguaRawImage[index];
        RectTransform rect = baguaIcon.GetComponent<RectTransform>();
        commandString += index.ToString();
        //Dotween动画
        baguaIcon.color = new Color(1, 1, 1, 1);
        rect.localScale = Vector3.one * 1f;
        rect.DOScale(new Vector3(0.15f, 0.15f, 0f), 0.3f).SetEase(Ease.InQuad);
        baguaIcon.DOFade(0.25f, 0.35f).SetEase(Ease.InQuad).onComplete =
                delegate ()
                {
                    baguaIcon.gameObject.SetActive(false);
                };
    }
    void CommandNameCheck() 
    {
        if (baGuaManager.baguasHolder.Count >= 2)
        {
            commandName.gameObject.SetActive(true);
            if (commandString == "23" || commandString == "32") //治疗
            {
                commandName.text = "Heal";
            }
            else if (commandString == "03" || commandString == "30") //火球
            {
                commandName.text = "Fire";
            }
            else
            {
                commandName.text = "Fail Match";
            }
        }
        else 
        {
            commandName.gameObject.SetActive(false);
        }
    }
}
