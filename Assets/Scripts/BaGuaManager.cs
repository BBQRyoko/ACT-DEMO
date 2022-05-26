using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BaGuaManager : MonoBehaviour
{
    GameManager gameManager;
    InputManager inputManager;
    PlayerManager playerManager;
    PlayerStats playerStats;
    PlayerInventory playerInventory;
    AnimatorManager animatorManager;

    Sample_VFX sample_VFX_Ability;
    public List<int> commandHolder = new List<int>();
    public int curPiviot;
    public Vector2 curPos;
    public GameObject realPiviot;

    [Header("阴阳槽相关")]
    public float curYin;
    [SerializeField] Image yinChargeSlot;
    public float curYang;
    [SerializeField] Image yangChargeSlot;

    [Header("八卦技能相关")]
    [SerializeField] GameObject BaGuaZhen;
    public Texture[] baguaRawImage;
    public GameObject[] baguaGameobjectPrefabs;

    public int commandSlotNum = 2;
    public List<int> baguasHolder = new List<int>();
    [SerializeField] GameObject baguaCharacterPrefab;
    [SerializeField] Transform baguaCharacterPosHolder;
    string commandString;
    bool isCommandActive;

    [Header("技能buff相关")]
    [SerializeField] List<GameObject> buffList = new List<GameObject>();

    [Header("能量相关")]
    public int energyGuage;
    public float curEnergyCharge;
    [SerializeField] Image energyChargeSlot;
    [SerializeField] Image energyGuage_1;
    [SerializeField] Image energyGuage_2;
    [SerializeField] Image energyGuage_3;
    [SerializeField] GameObject liCover;

    [Header("ui")]
    [SerializeField] GameObject fireballCheatSheet;

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        playerManager = GetComponent<PlayerManager>();
        playerStats = GetComponent<PlayerStats>();
        playerInventory = GetComponent<PlayerInventory>();
        inputManager = GetComponent<InputManager>();
        animatorManager = GetComponentInChildren<AnimatorManager>();
        sample_VFX_Ability = GetComponentInChildren<Sample_VFX>();
        curPos = realPiviot.transform.position;
    }
    void Update()
    {
        YinYangControl();
        //EnergySourceControl();
        BaguaPanelActive();
        BaguaAbilityController();
    }
    void BaguaPanelActive()
    {
        if (inputManager.baGua_Input && !isCommandActive && !playerManager.gameStart)
        {
            BaGuaZhen.SetActive(true);
            gameManager.GameSlowDown();
            realPiviot.transform.position = new Vector2(curPos.x + (inputManager.baguaInputX * 200), curPos.y + (inputManager.baguaInputY * 200));
            if (commandHolder.Count <= 3)
            {
                if (inputManager.baguaInputX >= 0.99 && inputManager.baguaInputY >= -0.13 && inputManager.baguaInputY <= 0.13)
                {
                }
                else if (inputManager.baguaInputX <= -0.99 && inputManager.baguaInputY >= -0.13 && inputManager.baguaInputY <= 0.13)
                {
                }
                else if (inputManager.baguaInputY >= 0.99 && inputManager.baguaInputX >= -0.13 && inputManager.baguaInputX <= 0.13)
                {
                }
                else if (inputManager.baguaInputY <= -0.99 && inputManager.baguaInputX >= -0.13 && inputManager.baguaInputX <= 0.13)
                {

                }
                //else if (inputManager.baguaInputX > 0.61 && inputManager.baguaInputX < 0.79 && inputManager.baguaInputY > 0.61 && inputManager.baguaInputX < 0.79)
                //{
                //    BaGuaCommand(1);
                //}
                //else if (inputManager.baguaInputX < -0.61 && inputManager.baguaInputX > -0.79 && inputManager.baguaInputY > 0.61 && inputManager.baguaInputX < 0.79)
                //{
                //    BaGuaCommand(7);
                //}
                //else if (inputManager.baguaInputX > 0.61 && inputManager.baguaInputX < 0.79 && inputManager.baguaInputY < -0.61 && inputManager.baguaInputX > -0.79)
                //{
                //    BaGuaCommand(3);
                //}
                //else if (inputManager.baguaInputX < -0.61 && inputManager.baguaInputX > -0.79 && inputManager.baguaInputY < -0.61 && inputManager.baguaInputX > -0.79)
                //{
                //    BaGuaCommand(5);
                //}
            }

            if (inputManager.lockOn_Input)
            {
                commandHolder.Clear();
            }
        }
        else
        {
            //BaGuaZhen.SetActive(false);
            gameManager.Resume();
            if (commandHolder.Count >= 1)
            {
                isCommandActive = true;
            }
            else
            {
                if (!isCommandActive)
                {
                    commandHolder.Clear();
                }
            }
        }
    }
    void EnergySourceControl()
    {
        energyChargeSlot.fillAmount = curEnergyCharge / 100;

        if (curEnergyCharge >= 100)
        {
            if (energyGuage == 3)
            {
                curEnergyCharge = 100;
            }
            else
            {
                curEnergyCharge -= 100;
                energyGuage += 1;
            }
        }

        if (energyGuage == 1)
        {
            energyGuage_1.fillAmount = 1;
            energyGuage_2.fillAmount = 0;
            energyGuage_3.fillAmount = 0;
        }
        else if (energyGuage == 2)
        {
            energyGuage_1.fillAmount = 1;
            energyGuage_2.fillAmount = 1;
            energyGuage_3.fillAmount = 0;
        }
        else if (energyGuage == 3)
        {
            energyGuage_1.fillAmount = 1;
            energyGuage_2.fillAmount = 1;
            energyGuage_3.fillAmount = 1;
        }
        else
        {
            energyGuage_1.fillAmount = 0;
            energyGuage_2.fillAmount = 0;
            energyGuage_3.fillAmount = 0;
        }
    }

    //New 
    public void AddBaguaCommand(int baguaIndex)
    {
        baguasHolder.Add(baguaIndex);
    }
    public void BaguaCasting()
    {
        if (baguasHolder.Count >= 2) 
        {
            isCommandActive = true;
            for (int i = 0; i < baguasHolder.Count; i++)
            {
                GameObject baguaCharacter = Instantiate(baguaCharacterPrefab, baguaCharacterPosHolder);
                RectTransform rect = baguaCharacter.GetComponent<RectTransform>();
                RawImage rawImage = baguaCharacter.GetComponent<RawImage>();

                rawImage.texture = baguaRawImage[baguasHolder[i]];
                rect.localScale = Vector3.zero;
                rect.DOScale(Vector3.one * 8f, 0.65f).SetEase(Ease.OutQuad).SetDelay(0.45f * i);
                commandString += baguasHolder[i].ToString();
                rawImage.DOFade(0, .6f).SetEase(Ease.OutQuad).SetDelay(0.45f * i).onComplete =
                    delegate ()
                    {
                        Destroy(baguaCharacter);
                    };
            }
        }
    }
    void BaguaAbilityController()
    {
        if (isCommandActive && baguaCharacterPosHolder.childCount == 0)
        {
            if (commandString == "23" || commandString == "32") //治疗
            {
                sample_VFX_Ability.curVFX_List[0].Play();
                playerStats.currHealth += 100;
                if (playerStats.currHealth >= playerStats.maxHealth)
                {
                    playerStats.currHealth = playerStats.maxHealth;
                }
                playerStats.healthBar.SetCurrentHealth(playerStats.currHealth);
                energyGuage -= 1;
                animatorManager.generalAudio.volume = 0.08f;
                animatorManager.generalAudio.clip = animatorManager.sample_SFX.Bagua_SFX_List[1];
                animatorManager.generalAudio.Play();
            }
            else if (commandString == "27" || commandString == "72") //风buff
            {
                Instantiate(buffList[1], transform, false);
                animatorManager.generalAudio.volume = 0.08f;
                animatorManager.generalAudio.clip = animatorManager.sample_SFX.Bagua_SFX_List[1];
                animatorManager.generalAudio.Play();
            }
            else if (commandString == "02" || commandString == "20") //火buff
            {
                Instantiate(buffList[0], transform, false);
                animatorManager.generalAudio.volume = 0.08f;
                animatorManager.generalAudio.clip = animatorManager.sample_SFX.Bagua_SFX_List[1];
                animatorManager.generalAudio.Play();
            }
            else if (commandString == "03" || commandString == "30") //火球
            {
                animatorManager.PlayTargetAnimation("FireBall", true, true);
                energyGuage -= 1;
                animatorManager.generalAudio.volume = 0.08f;
                animatorManager.generalAudio.clip = animatorManager.sample_SFX.Bagua_SFX_List[2];
                animatorManager.generalAudio.Play();
            }
            else if (commandString == "73" || commandString == "37") //龙卷
            {
                animatorManager.PlayTargetAnimation("Tornado", true, true);
                energyGuage -= 1;
                animatorManager.generalAudio.volume = 0.08f;
                animatorManager.generalAudio.clip = animatorManager.sample_SFX.Bagua_SFX_List[2];
                animatorManager.generalAudio.Play();
            }

            baguasHolder.Clear();
            commandString = null;
            isCommandActive = false;
        }
    }
    void YinYangControl() 
    {
        yinChargeSlot.fillAmount = curYin / 100;
        yangChargeSlot.fillAmount = curYang / 100;
        if (curYin >= 100 && curYang >= 100) 
        {
            playerManager.yinYangAbilityOn = true;
        }
    }
    public void YinYangChargeUp(float chargeValue) 
    {
        if (playerInventory.currentWeaponIndex == 0)
        {
            curYin += chargeValue;
            if (curYin >= 100) curYin = 100;
        }
        else 
        {
            curYang += chargeValue;
            if (curYang >= 100) curYang = 100;
        }
    }
}
