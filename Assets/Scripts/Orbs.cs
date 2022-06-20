using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbs : MonoBehaviour
{
    AnimatorManager animatorManager;
    public enum OrbsType{ health, energy, arrow};
    public OrbsType orb;
    [SerializeField] int restoreAmount;
    [SerializeField] AudioClip audioClip;

    private void Start()
    {
        animatorManager = FindObjectOfType<AnimatorManager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            if (orb == OrbsType.health)
            {
                PlayerStats playerStats = other.GetComponent<PlayerStats>();
                if (playerStats.currHealth < playerStats.maxHealth) 
                {
                    playerStats.currHealth += restoreAmount;
                    animatorManager.generalAudio.volume = 0.2f;
                    animatorManager.generalAudio.clip = audioClip;
                    animatorManager.generalAudio.Play();
                    Destroy(gameObject.transform.parent.gameObject);
                }
            }
            else if (orb == OrbsType.energy) 
            {
                BaGuaManager baGuaManager = other.GetComponent<BaGuaManager>();
                if (baGuaManager.curEnergyCharge < 300 )
                {
                    baGuaManager.curEnergyCharge += (float)restoreAmount;
                    animatorManager.generalAudio.volume = 0.2f;
                    animatorManager.generalAudio.clip = audioClip;
                    animatorManager.generalAudio.Play();
                    Destroy(gameObject.transform.parent.gameObject);
                }
            }
        }
    }
}
