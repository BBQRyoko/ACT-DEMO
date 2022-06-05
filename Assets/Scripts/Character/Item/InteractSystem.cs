using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractSystem : MonoBehaviour
{
    CameraManager cameraManager;
    PlayerManager playerManager;

    public bool promptOn;

    public float curInteractTime;
    public float defaultInteractTime;

    [SerializeField] string interactText;

    // Start is called before the first frame update
    void Awake()
    {
        cameraManager = FindObjectOfType<CameraManager>();
        playerManager = FindObjectOfType<PlayerManager>();
    }
    // Update is called once per frame
    void Update()
    {
        InputManager inputManager = playerManager.GetComponent<InputManager>();

        if (promptOn)
        {
            if (inputManager.interact_Input)
            {
                curInteractTime += Time.deltaTime;
                if (curInteractTime >= defaultInteractTime)
                {
                    Interact();
                }
            }
            else 
            {
                if(curInteractTime>0) curInteractTime -= Time.deltaTime;
            }
        }
    }

    public void HandleInteractUI(InteractSystem interact) 
    {
        if (!promptOn)
        {
            cameraManager.GenerateInteractPrompt(interact, interactText);
            promptOn = true;
        }
    }
    public virtual void Interact() 
    {
        promptOn = false;
        playerManager.interactObject = false;
        curInteractTime = 0;
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerManager>().inInteractTrigger = true;
            HandleInteractUI(this);
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
