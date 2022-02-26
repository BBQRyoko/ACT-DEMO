using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyFillingUI : MonoBehaviour
{
    public enum fillingType {health, alert};
    public fillingType fillUIType;
    public EnemyManager enemyManager;
    public Slider slider;

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
    }
    public void SetEnemyManager(EnemyManager enemyManager) 
    {
        this.enemyManager = enemyManager;
        if (fillUIType == fillingType.health && enemyManager != null)
        {
            slider.maxValue = enemyManager.GetComponent<EnemyStats>().maxHealth;
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
