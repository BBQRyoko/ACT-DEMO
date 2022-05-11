using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    PlayerManager playerManager;
    public GameObject aimingCorsshair;

    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
    }
}
