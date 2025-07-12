using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEffecsManager : MonoBehaviour
{
    [Header("Damage VFX")]
    public GameObject regularBloodVFX;
    public GameObject deadBloodVFX;
    public GameObject regularParryHitVFX;
    public GameObject parryBreakHitVFX;
    // Start is called before the first frame update
    public virtual void PlayHitBloodVFX(Vector3 hitPos, float damage, bool isDead = false) 
    {
        if (!isDead)
        {
            if (damage < 20)
            {
                return;
            }
            else if (damage >= 100)
            {
                GameObject blood = Instantiate(regularBloodVFX, hitPos, Quaternion.identity);
                blood.transform.localScale = Vector3.one * 1.2f * 1.5f;

            }
            else
            {
                GameObject blood = Instantiate(regularBloodVFX, hitPos, Quaternion.identity);
                blood.transform.localScale = Vector3.one * ((damage - 20) / 80 * 0.55f + 0.65f) * 1.5f;
            }
        }
        else 
        {
            GameObject dead = Instantiate(deadBloodVFX, hitPos, Quaternion.identity);
        }
    }
    public virtual void PlayHitParryVFX(Vector3 hitPos, bool isBreak = false)
    {
        if (isBreak) 
        {
            GameObject parrySpark = Instantiate(parryBreakHitVFX, hitPos, Quaternion.identity);
        }
        else 
        {
            GameObject parrySpark = Instantiate(regularParryHitVFX, hitPos, Quaternion.identity);
        }
    }
}
