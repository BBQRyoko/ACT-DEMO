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
    [SerializeField] AudioSource generalAudio;
    [SerializeField] Sample_SFX sfxList;

    Sample_VFX sample_VFX_Ability;
    public Vector2 curPos;
    public GameObject realPiviot;

    [Header("阴阳槽相关")]

    public float curYin;
    [SerializeField] Image yinChargeSlot;
    public float curYang;
    [SerializeField] Image yangChargeSlot;
    bool ultimateTeach;
    public GameObject ultimateTutorial;
    public bool tutorialUp;


    [Header("八卦技能相关")]
    public GuidePopup switchAttackTutorial;
    [SerializeField] GameObject BaGuaZhen;
    public Texture[] baguaRawImage;
    public List<GameObject> baguaGameobjectPrefabs = new List<GameObject>();

    public int commandSlotNum = 2;
    public List<int> baguasHolder = new List<int>();
    [SerializeField] GameObject baguaCharacterPrefab;
    [SerializeField] Transform baguaCharacterPosHolder;
    string commandString;
    bool isCommandActive;

    [Header("技能buff相关")]
    public List<GameObject> buffList = new List<GameObject>();

    [Header("能量相关")]
    public float curEnergyCharge;
    [SerializeField] Image energyChargeSlot;
    [SerializeField] Image energyGuage_1;
    [SerializeField] Image energyGuage_2;
    [SerializeField] Image energyGuage_3;
    public bool isSwitchAttack; 

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
        curPos = realPiviot.GetComponent<RectTransform>().position;
    }
    void Update()
    {
        YinYangControl();
        EnergySourceControl();
        BaguaPanelActive();
        BaguaAbilityController();
    }
    void BaguaPanelActive()
    {
        if (inputManager.baGua_Input && !isCommandActive && !playerManager.gameStart && curEnergyCharge >= 100)
        {
            BaGuaZhen.SetActive(true);
            gameManager.GameSlowDown();

            float x = curPos.x + (inputManager.baguaInputX * 220 * (curPos.x / 960));
            float y = curPos.y + (inputManager.baguaInputY * 220 * (curPos.x / 960));
            RectTransform rect = realPiviot.GetComponent<RectTransform>();
            rect.position = new Vector2(x, y);
        }
        else
        {
            gameManager.Resume();
        }
    }
    void EnergySourceControl()
    {
        energyChargeSlot.fillAmount = curEnergyCharge / 300;

        if (curEnergyCharge >= 300)
        {
            curEnergyCharge = 300;
            energyGuage_1.fillAmount = 1;
            energyGuage_2.fillAmount = 1;
            energyGuage_3.fillAmount = 1;
        }
        else if (curEnergyCharge >= 200)
        {
            energyGuage_1.fillAmount = 1;
            energyGuage_2.fillAmount = 1;
            energyGuage_3.fillAmount = 0;
        }
        else if (curEnergyCharge >= 100)
        {
            energyGuage_1.fillAmount = 1;
            energyGuage_2.fillAmount = 0;
            energyGuage_3.fillAmount = 0;
        }
        else 
        {
            energyGuage_1.fillAmount = 0;
            energyGuage_2.fillAmount = 0;
            energyGuage_3.fillAmount = 0;
        }
    }
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
                if(!playerManager.isDead) playerStats.currHealth += 80;
                if (playerStats.currHealth >= playerStats.maxHealth)
                {
                    playerStats.currHealth = playerStats.maxHealth;
                }
                playerStats.healthBar.SetCurrentHealth(playerStats.currHealth);
                curEnergyCharge -= 100;
                animatorManager.generalAudio.volume = 0.08f;
                animatorManager.generalAudio.clip = animatorManager.sample_SFX.Bagua_SFX_List[1];
                animatorManager.generalAudio.Play();
            }
            else if (commandString == "27" || commandString == "72") //风buff
            {
                Instantiate(buffList[1], transform, false);
                curEnergyCharge -= 100;
                animatorManager.generalAudio.volume = 0.08f;
                animatorManager.generalAudio.clip = animatorManager.sample_SFX.Bagua_SFX_List[1];
                animatorManager.generalAudio.Play();
            }
            else if (commandString == "02" || commandString == "20") //火buff
            {
                Instantiate(buffList[0], transform, false);
                curEnergyCharge -= 100;
                animatorManager.generalAudio.volume = 0.08f;
                animatorManager.generalAudio.clip = animatorManager.sample_SFX.Bagua_SFX_List[1];
                animatorManager.generalAudio.Play();
            }
            else if (commandString == "03" || commandString == "30") //火球
            {
                animatorManager.PlayTargetAnimation("FireBall", true, true);
                curEnergyCharge -= 100;
                animatorManager.generalAudio.volume = 0.08f;
                animatorManager.generalAudio.clip = animatorManager.sample_SFX.Bagua_SFX_List[2];
                animatorManager.generalAudio.Play();
            }
            else if (commandString == "73" || commandString == "37") //龙卷
            {
                animatorManager.PlayTargetAnimation("Tornado", true, true);
                curEnergyCharge -= 100;
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
        yinChargeSlot.fillAmount = curYin / 80;
        yangChargeSlot.fillAmount = curYang / 80;
        if (curYin >= 80 && curYang >= 80 && !playerManager.yinYangAbilityOn) 
        {
            if (!ultimateTeach) 
            {
                ultimateTeach = true;
                ultimateTutorial.SetActive(true);
                tutorialUp = true;
                Destroy(ultimateTutorial.gameObject, 10f);
            }
            generalAudio.clip = sfxList.Bagua_SFX_List[0];
            generalAudio.Play();
            playerManager.yinYangAbilityOn = true;
        }
    }
    public void YinYangChargeUp(float chargeValue) 
    {
        if (playerInventory.currentWeaponIndex == 0)
        {
            curYin += chargeValue;
            if (curYin >= 80) curYin = 80;
        }
        else 
        {
            curYang += chargeValue;
            if (curYang >= 80) curYang = 80;
        }
    }
}
