using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AT_Field : MonoBehaviour
{
    [SerializeField] List<EnemyManager> enemyManagerList;
    [SerializeField] float filedTime;
    [SerializeField] float counterTimer = 0.5f;
    [SerializeField] float filedRadius;

    private void Start()
    {
        counterTimer = filedTime;
    }

    private void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, filedRadius);
        if (colliders.Length > 0) 
        {
            for (int i = 0; i < colliders.Length; i++) 
            {
                EnemyManager enemyManager = colliders[i].GetComponent<EnemyManager>();
                if (enemyManager != null && !enemyManager.isTaijied) 
                {
                    enemyManagerList.Add(enemyManager);
                }
            }
        }

        if (transform.localScale.x < 8)
        {
            transform.localScale += new Vector3(1, 0, 1) * 8f * Time.deltaTime;
        }
        else
        {
            counterTimer -= Time.deltaTime;
        }

        if (enemyManagerList != null) 
        {
            foreach (EnemyManager enemy in enemyManagerList) 
            {
                enemy.GetComponentInChildren<Animator>().speed = 0.1f;
                enemy.GetComponentInChildren<EnemyWeaponSlotManager>().CloseWeaponDamageCollider();
                enemy.GetComponentInChildren<Animator>().SetBool("canReset", true);
                enemy.GetComponent<Rigidbody>().isKinematic = true;
                enemy.isTaijied = true;
            }
        }

        if (counterTimer <= 0) 
        {
            counterTimer = 0;
            foreach (EnemyManager enemy in enemyManagerList)
            {
                enemy.GetComponentInChildren<Animator>().speed = 1f;
                enemy.GetComponent<Rigidbody>().isKinematic = false;
                enemy.isTaijied = false;
            }
            Destroy(gameObject);
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, filedRadius);
    }
}
