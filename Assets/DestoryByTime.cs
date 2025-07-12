using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestoryByTime : MonoBehaviour
{
    [SerializeField] float duration = 8f;
    private void Start()
    {
        Destroy(gameObject, duration);
    }
}
