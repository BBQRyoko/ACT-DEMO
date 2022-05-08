using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKillRequireDoor : MonoBehaviour
{
    [SerializeField] EnemyManager requiredEnemy;
    Animator animator;
    int enemyCount;
    bool doorIsOpened;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (requiredEnemy.isDead && !doorIsOpened)
        {
            animator.SetTrigger("DoorOpen");
            gameObject.GetComponent<BoxCollider>().enabled = false;
            doorIsOpened = true;
        }
    }
}
