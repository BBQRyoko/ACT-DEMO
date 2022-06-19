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
    public string commandString;
    [SerializeField] GameObject noAbilityIcon;
    [SerializeField] GameObject abilityIcon;
    [SerializeField] Image abilityIcon_Image;

    [SerializeField] Sprite[] abilityIcons;
    [SerializeField] GameObject[] arrows;

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
        noAbilityIcon.SetActive(true);
        actived = true;
    }
    void PanelClose() 
    {
        for (int i = 0; i < baGuaManager.baguaGameobjectPrefabs.Count; i++)
        {
            RectTransform rect = baguaList[i].GetComponent<RectTransform>();
            GameObject entry = baguaList[i].gameObject;

            rect.DOScale(Vector3.zero, 0.15f).SetEase(Ease.OutQuad).onComplete =
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

            rect.DOScale(Vector3.zero, 0.15f).SetEase(Ease.OutQuad).onComplete =
                delegate ()
                {
                    Destroy(entry);
                    gameObject.SetActive(false);
                };
        }
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
        commandString = null;
        abilityIcon.SetActive(false);
        baGuaManager.openTimer = 0.5f;
        foreach (GameObject arrow in arrows) arrow.SetActive(false);
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
            rect.position = new Vector3(x, y, 0);
            rect.DOScale(Vector3.one, 0f);
            rect.DOAnchorPos(new Vector3(x, y, 0f), 0f).onComplete =
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

            rect.DORotateQuaternion(Quaternion.Euler(0, 0, radiansOfSeparation * (i+1)), 0f);
        }
    }
    public void CommandFilling(int index, int dir)
    {
        baguaIcon.gameObject.SetActive(true);
        baguaIcon.texture = baGuaManager.baguaRawImage[index];
        arrows[dir].SetActive(true);
        RectTransform rect = baguaIcon.GetComponent<RectTransform>();
        commandString += index.ToString();
        //Dotween动画
        baguaIcon.color = new Color(1, 1, 1, 1);
        rect.localScale = Vector3.one * 1f;
        rect.DOScale(new Vector3(0.15f, 0.15f, 0f), 0.15f).SetEase(Ease.InQuad);
        baguaIcon.DOFade(0.25f, 0.2f).SetEase(Ease.InQuad).onComplete =
                delegate ()
                {
                    baguaIcon.gameObject.SetActive(false);
                };
    }
    void CommandNameCheck() 
    {
        if (baGuaManager.baguasHolder.Count >= 2)
        {
            if (commandString == "23" || commandString == "32") //治疗
            {
                abilityIcon.SetActive(true);
                noAbilityIcon.SetActive(false);
                abilityIcon_Image.sprite = abilityIcons[0];
            }
            else if (commandString == "03" || commandString == "30") //火球
            {
                abilityIcon.SetActive(true);
                noAbilityIcon.SetActive(false);
                abilityIcon_Image.sprite = abilityIcons[1];
            }
            else if (commandString == "02" || commandString == "20") //攻击
            {
                abilityIcon.SetActive(true);
                noAbilityIcon.SetActive(false);
                abilityIcon_Image.sprite = abilityIcons[2];
            }
            else if (commandString == "73" || commandString == "37") //龙卷
            {
                abilityIcon.SetActive(true);
                noAbilityIcon.SetActive(false);
                abilityIcon_Image.sprite = abilityIcons[3];
            }
            else if (commandString == "72" || commandString == "27") //风buff
            {
                abilityIcon.SetActive(true);
                noAbilityIcon.SetActive(false);
                abilityIcon_Image.sprite = abilityIcons[4];
            }
            else
            {
                abilityIcon.SetActive(false);
                noAbilityIcon.SetActive(true);
            }
        }
    }
}
