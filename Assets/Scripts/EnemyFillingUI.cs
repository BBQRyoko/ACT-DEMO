using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyFillingUI : MonoBehaviour
{
    public enum fillingType {health, alert, stamina};
    public fillingType fillUIType;
    public EnemyManager enemyManager;
    public Slider slider;
    [SerializeField] EnemyFillingUI staminaFillingUI;

    private void Update()
    {
        if (fillUIType == fillingType.health)
        {
            slider.value = enemyManager.GetComponent<EnemyStats>().currHealth;
        }
        else if (fillUIType == fillingType.alert)
        {
            slider.value = enemyManager.alertTimer;
        }
        else if (fillUIType == fillingType.stamina) 
        {
            slider.value = GetComponentInParent<EnemyFillingUI>().enemyManager.GetComponent<EnemyStats>().currStamina;
        }

        if (slider.value <= 0)
        {
            if (fillUIType == fillingType.health)
            {
                gameObject.SetActive(false);
            }
            else if (fillUIType == fillingType.alert && !enemyManager.curTarget)
            {
                Destroy(gameObject);
            }
        }

        if (fillUIType == fillingType.health && !enemyManager.beenLocked) 
        {
            Destroy(gameObject, 1.5f);
        }

        if (fillUIType == fillingType.alert)
        {
            if (enemyManager.isDead || (enemyManager.curTarget && !enemyManager.isAlerting))
            {
                Destroy(gameObject);
            }
        }

    }
    public void SetEnemyManager(EnemyManager enemyManager) 
    {
        this.enemyManager = enemyManager;
        if (fillUIType == fillingType.health && enemyManager != null)
        {
            slider.maxValue = enemyManager.GetComponent<EnemyStats>().maxHealth;
            staminaFillingUI.slider.maxValue = enemyManager.GetComponent<EnemyStats>().maxStamina;
            staminaFillingUI.enemyManager = enemyManager;
        }
        else if (fillUIType == fillingType.alert)
        {
            slider.maxValue = 5f;
        }
    }

    private void LateUpdate()
    {
        if (fillUIType == fillingType.health)
        {
            transform.position = Camera.main.WorldToScreenPoint(new Vector3(enemyManager.targetMarkTransform.position.x, enemyManager.targetMarkTransform.position.y + 1f, enemyManager.targetMarkTransform.position.z));
        }
        else if (fillUIType == fillingType.alert)
        {
            transform.position = Camera.main.WorldToScreenPoint(new Vector3(enemyManager.targetMarkTransform.position.x, enemyManager.targetMarkTransform.position.y + 1.5f, enemyManager.targetMarkTransform.position.z));
        }
    }
}
