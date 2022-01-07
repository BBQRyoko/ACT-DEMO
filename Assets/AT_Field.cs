using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AT_Field : MonoBehaviour
{
    [SerializeField] List<EnemyAnimatorManager> enemyAnimatorList;
    [SerializeField] float filedTime;
    [SerializeField] float counterTimer = 0.5f;

    private void Start()
    {
        counterTimer = filedTime;
    }

    private void Update()
    {
        if (enemyAnimatorList != null) 
        {
            foreach (EnemyAnimatorManager enemy in enemyAnimatorList) 
            {
                if (enemy.GetComponent<Animator>() != null) 
                {
                    Debug.Log("The World");
                    enemy.GetComponent<Animator>().speed = 0.1f;
                    enemy.GetComponentInParent<Rigidbody>().isKinematic = true;
                }
            }
        }

        if (transform.localScale.x < 15)
        {
            transform.localScale += new Vector3(1,0,1) * 7.5f * Time.deltaTime;
        }
        else 
        {
            counterTimer -= Time.deltaTime;
        }

        if (counterTimer <= 0) 
        {
            counterTimer = 0;
            foreach (EnemyAnimatorManager enemy in enemyAnimatorList)
            {
                enemy.animator.speed = 1f;
                enemy.GetComponentInParent<Rigidbody>().isKinematic = false;
            }

            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy")) 
        {
            EnemyAnimatorManager enemyAnimatorManager = other.GetComponentInChildren<EnemyAnimatorManager>();

            if (enemyAnimatorManager != null) 
            {
                enemyAnimatorList.Add(enemyAnimatorManager);
            }
        }
    }
}
