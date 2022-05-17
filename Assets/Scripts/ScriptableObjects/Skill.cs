using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/New Skill")]
public class Skill : ScriptableObject
{
    public string skillName;
    public bool isImmuAttack = false;
    public int damagePoint;
    public int tenacityDamagePoint;
    public int staminaCost = 15;
    public int energyRestore = 20;
    public int pauseDuration = 10;
    //public Animation animation;
    public ParticleSystem[] VFXs;
    public AudioClip[] SFXs;

    [Header("飞行道具参数")]
    public bool isFlyingObject;
    public float maxSpeed = 20f;
}
