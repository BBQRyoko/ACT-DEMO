using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuidePopup : MonoBehaviour
{
    //当前只触发教程
    [SerializeField] GameObject guide;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            guide.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            guide.SetActive(false);
        }
    }
}
