using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyFillingUI : MonoBehaviour
{
    public EnemyManager enemyManager;
    public Slider slider;

    private void Update()
    {
        slider.value = enemyManager.alertTimer;

        if (slider.value <= 0) 
        {
            Destroy(gameObject);
        }
    }

    public void SetEnemyManager(EnemyManager enemyManager) 
    {
        this.enemyManager = enemyManager;
        slider.maxValue = 5f;
    }

    private void LateUpdate()
    {
        transform.position = Camera.main.WorldToScreenPoint(new Vector3(enemyManager.targetMarkTransform.position.x, enemyManager.targetMarkTransform.position.y + 1.5f, enemyManager.targetMarkTransform.position.z));
    }
}
