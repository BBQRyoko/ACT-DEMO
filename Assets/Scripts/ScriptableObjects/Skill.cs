using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/New Skill")]
public class Skill : ScriptableObject
{
    public string skillName;
    public int skillType; //0 - 单发, 1 - 蓄力
    //public Animation animation;
    public ParticleSystem[] VFXs;
    public AudioClip[] SFXs;
}
