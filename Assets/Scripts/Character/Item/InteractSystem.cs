using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractSystem : MonoBehaviour
{
    CameraManager cameraManager;
    [SerializeField] PlayerManager playerManager;
    public GameObject interactPrompt;
    public bool promptOn;
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

    private void HandleInteractUI() 
    {
    
    }
    public virtual void Interact() 
    {
        promptOn = false;
        playerManager.interactObject = false;
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerManager>().inInteractTrigger = true;
            promptOn = true;
        }
    }

    public virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerManager>().inInteractTrigger = false; 
            promptOn = false;
        }
    }
}
