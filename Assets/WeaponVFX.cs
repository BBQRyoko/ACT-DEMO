using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponVFX : MonoBehaviour
{
    public ParticleSystem regularWeaponTrail;

    public void PlayWeaponVFX() 
    {
        regularWeaponTrail.Stop();
        regularWeaponTrail.Play();
    }
}
