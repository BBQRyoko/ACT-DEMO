using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DangerMark : MonoBehaviour
{
    public EnemyManager enemyManager;
    public Image dangerMark_Prefab;
    Image dangerMark;

    private void Awake()
    {
        dangerMark = Instantiate(dangerMark_Prefab, FindObjectOfType<Canvas>().transform).GetComponent<Image>();
    }
    void Start()
    {
        enemyManager = GetComponentInParent<EnemyManager>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Camera.main.WorldToScreenPoint(new Vector3(enemyManager.transform.position.x, enemyManager.transform.position.y, enemyManager.transform.position.z));
    }
}
