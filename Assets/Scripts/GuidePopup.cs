using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GuidePopup : MonoBehaviour
{
    //当前只触发教程
    [SerializeField] GameObject guide;

    //特殊教程
    bool playerEntered;
    [SerializeField] bool switchAttackTutorial;
    [SerializeField] GameObject teachingDummy;
    [SerializeField] GameObject fogwall1;
    [SerializeField] GameObject fogwall2;
    public int switchAttackNum;
    [SerializeField] TextMeshProUGUI attackCount;

    private void Update()
    {
        if (playerEntered && switchAttackTutorial) 
        {
            SwitchTutorial();
        }
    }
    void SwitchTutorial() 
    {
        //考虑一下玩家砍死重生的情况
        if (teachingDummy.GetComponentInChildren<EnemyStats>().currHealth <= 0) teachingDummy.GetComponentInChildren<EnemyStats>().currHealth = 100000;
        attackCount.text = switchAttackNum.ToString();
        teachingDummy.SetActive(true);
        fogwall1.SetActive(true);
        fogwall2.SetActive(true);
        if (switchAttackNum >= 3) 
        {
            teachingDummy.GetComponentInChildren<EnemyStats>().currHealth -= 10000000;
            guide.SetActive(false);
            Destroy(this.gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            guide.SetActive(true);
            if(switchAttackTutorial) playerEntered= true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            guide.SetActive(false);
        }
    }
}
