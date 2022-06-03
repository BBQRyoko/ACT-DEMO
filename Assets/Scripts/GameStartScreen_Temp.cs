using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartScreen_Temp : MonoBehaviour
{
    [SerializeField] AnimatorManager animatorManager;
    void Start()
    {
        animatorManager = FindObjectOfType<AnimatorManager>();
        animatorManager.GetComponentInParent<PlayerManager>().gameStart = true;
        animatorManager.PlayTargetAnimation("GameStartFall", true, true);
        Destroy(this.gameObject, 10f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
