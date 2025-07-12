using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sample_SFX : MonoBehaviour
{
    public AudioClip[] curSFX_List;
    public AudioClip[] hittedSFX_List;
    public AudioClip[] blockedSFX_List;
    public AudioClip blockFailedSFX;
    public AudioClip EquipSFX;
    public AudioClip ExecutionSFX;

    [Header("PlayerOnly")]
    public AudioClip[] Bagua_SFX_List;
    public AudioClip[] checkPoint_Heal;

    [Header("EnemyOnly")]
    public AudioClip EnemyCallingSFX;
}
