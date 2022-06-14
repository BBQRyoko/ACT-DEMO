using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/New Skill")]
public class Skill : ScriptableObject
{
    public string skillName;
    public bool isImmuAttack = false;
    public bool isHeavy;
    public float damagePoint;
    public float tenacityDamagePoint;
    public float staminaCost = 15;
    public float hitPoint = 3;
    public float energyRestore = 5;
    public int pauseDuration = 10;
    //public Animation animation;
    public ParticleSystem[] VFXs;
    public AudioClip[] SFXs;

    [Header("飞行道具参数")]
    public bool isFlyingObject;
    public float maxSpeed = 20f;
}
