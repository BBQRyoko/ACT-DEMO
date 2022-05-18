using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaGuaManager : MonoBehaviour
{
    PlayerManager playerManager;
    PlayerStats playerStats;
    PlayerInventory playerInventory;
    AnimatorManager animatorManager;
    [SerializeField] GameObject BaGuaZhen;
    Sample_VFX sample_VFX_Ability;
    InputManager inputManager;
    public List<int> commandHolder = new List<int>();
    public string commandString;
    public int curPiviot;

    public float curYin;
    [SerializeField] Image yinChargeSlot;
    public float curYang;
    [SerializeField] Image yangChargeSlot;

    public int energyGuage;
    public float curEnergyCharge;
    [SerializeField] Image energyChargeSlot;
    [SerializeField] Image energyGuage_1;
    [SerializeField] Image energyGuage_2;
    [SerializeField] Image energyGuage_3;
    [SerializeField] GameObject liCover;

    public Vector2 curPos;
    public GameObject realPiviot;

    [SerializeField] GameObject[] BaGuaText;
    [SerializeField] Transform spawnPos;
    [SerializeField] float spawnTimer;
    public bool isCommandActive;

    public bool healUnlock;
    public bool fireBallUnlock;
    public bool immuUnlock;
    public bool forthUnlock_Temp;

    [Header("ui")]
    [SerializeField] GameObject fireballCheatSheet;

    void Awake()
    {
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
        if (fireBallUnlock)
        {
            liCover.SetActive(false);
            fireballCheatSheet.SetActive(true);
        }
        else 
        {
            liCover.SetActive(true);
            fireballCheatSheet.SetActive(false);
        }
        YinYangControl();
        EnergySourceControl();
        if (inputManager.baGua_Input && !isCommandActive && !playerManager.gameStart)
        {
            BaGuaZhen.SetActive(true);
            realPiviot.transform.position = new Vector2(curPos.x + (inputManager.baguaInputX * 200), curPos.y + (inputManager.baguaInputY * 200));
            if (commandHolder.Count <= 3) 
            {
                if (inputManager.baguaInputX >= 0.99 && inputManager.baguaInputY >= -0.13 && inputManager.baguaInputY <= 0.13)
                {
                    BaGuaCommand(2);
                }
                else if (inputManager.baguaInputX <= -0.99 && inputManager.baguaInputY >= -0.13 && inputManager.baguaInputY <= 0.13 && immuUnlock)
                {
                    BaGuaCommand(6);
                }
                else if (inputManager.baguaInputY >= 0.99 && inputManager.baguaInputX >= -0.13 && inputManager.baguaInputX <= 0.13 && fireBallUnlock)
                {
                    BaGuaCommand(0);
                }
                else if (inputManager.baguaInputY <= -0.99 && inputManager.baguaInputX >= -0.13 && inputManager.baguaInputX <= 0.13 && forthUnlock_Temp)
                {
                    BaGuaCommand(4);
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
            BaGuaZhen.SetActive(false);
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

        if (isCommandActive) 
        {
            CommandActive();
        }
    }
    private void FixedUpdate()
    {
        spawnTimer -= Time.fixedDeltaTime;
        if (spawnTimer <= 0) 
        {
            spawnTimer = 0;
        }
    }
    void BaGuaCommand(int index)
    {
        if (commandHolder.Count == 0)
        {
            commandHolder.Add(index);
        }
        else if(commandHolder.Count <= 3)
        {
            if (index != commandHolder[commandHolder.Count - 1])
            {
                commandHolder.Add(index);
            }
        }
    }
    void CommandActive()
    {
        if (commandHolder.Count != 0)
        {
            if (spawnTimer <= 0) //生成字特效
            {
                GameObject baguaText = Instantiate(BaGuaText[commandHolder[0]], new Vector3(spawnPos.position.x, spawnPos.position.y, spawnPos.position.z), Quaternion.identity);
                Destroy(baguaText, 1f);
                spawnTimer = 0.45f;
                commandString += commandHolder[0].ToString();
                commandHolder.Remove(commandHolder[0]);
            }
        }
        else 
        {
            if (commandString == "2" && healUnlock)
            {
                //SFX
                if (energyGuage >= 1) 
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
            }
            else if (commandString == "0" && fireBallUnlock)
            {
                if (energyGuage >= 1)
                {
                    animatorManager.PlayTargetAnimation("FireBall", true, true);
                    energyGuage -= 1;
                    animatorManager.generalAudio.volume = 0.08f;
                    animatorManager.generalAudio.clip = animatorManager.sample_SFX.Bagua_SFX_List[2];
                    animatorManager.generalAudio.Play();
                }
            }
            //else if (commandString == "732" && immuUnlock)
            //{
            //    if (energyGuage >= 2)
            //    {
            //        Debug.Log("Immu");
            //    }
            //}
            else 
            {
                Debug.Log(commandString);
            }

            commandString = null;
            isCommandActive = false;
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
        else if(energyGuage == 2)
        {
            energyGuage_1.fillAmount = 1;
            energyGuage_2.fillAmount = 1;
            energyGuage_3.fillAmount = 0;
        }
        else if(energyGuage == 3)
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
