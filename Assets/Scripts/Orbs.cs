using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbs : MonoBehaviour
{
    public enum OrbsType{ health, energy, arrow};
    public OrbsType orb;
    [SerializeField] int restoreAmount;
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
                    Destroy(gameObject.transform.parent.gameObject);
                }
            }
            else if (orb == OrbsType.energy) 
            {
                BaGuaManager baGuaManager = other.GetComponent<BaGuaManager>();
                if (baGuaManager.curEnergyCharge < 300 )
                {
                    baGuaManager.curEnergyCharge += (float)restoreAmount;
                    Destroy(gameObject.transform.parent.gameObject);
                }
            }
        }
    }
}
