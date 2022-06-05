using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractPromptUI : MonoBehaviour
{
    public InteractSystem interact;
    public Image imageSlider;
    [SerializeField] TextMeshProUGUI textHolder;

    // Update is called once per frame
    void Update()
    {
        if (interact) 
        {
            imageSlider.fillAmount = interact.curInteractTime / interact.defaultInteractTime;
            if (!interact.promptOn) 
            {
                DestroyCurrentInteractObject();
            }
        }
    }

    public void SetInteractObject(InteractSystem interact, string interactText) 
    {
        this.interact = interact;
        imageSlider.fillAmount = interact.curInteractTime / interact.defaultInteractTime;
        textHolder.text = interactText;
    }

    public void DestroyCurrentInteractObject() 
    {
        Destroy(this.gameObject);
    }

    private void LateUpdate()
    {
        Vector3 pos = interact.transform.position;
        transform.position = Camera.main.WorldToScreenPoint(pos);
    }
}
