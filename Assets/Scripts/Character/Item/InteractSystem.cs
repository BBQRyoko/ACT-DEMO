using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractSystem : MonoBehaviour
{
    [SerializeField] PlayerManager playerManager;
    public GameObject interactPrompt;
    bool promptOn;
    // Start is called before the first frame update
    void Awake()
    {
        playerManager = FindObjectOfType<PlayerManager>();
    }
    // Update is called once per frame
    void Update()
    {
        if (promptOn)
        {
            interactPrompt.SetActive(true);
            if (playerManager.interactObject)
            {
                Interact();
                playerManager.interactObject = false;
            }
        }
        else
        {
            interactPrompt.SetActive(false);
        }
    }
    public virtual void Interact() 
    {
        promptOn = false;
        playerManager.interactObject = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerManager>().inInteractTrigger = true;
            promptOn = true;
        }
        else
        {
            promptOn = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerManager>().inInteractTrigger = false; 
            promptOn = false;
        }
    }
}
