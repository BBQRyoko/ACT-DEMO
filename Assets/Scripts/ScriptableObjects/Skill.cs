using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/New Skill")]
public class Skill : ScriptableObject
{
    public string skillName;
    public bool isImmuAttack = false;
    public int skillType; //0 - 单发, 1 - 蓄力 (暂时废弃)
    public int damagePoint;
    public int staminaCost = 15;
    public int energyRestore = 20;
    public int pauseDuration = 10;
    //public Animation animation;
    public ParticleSystem[] VFXs;
    public AudioClip[] SFXs;
}
