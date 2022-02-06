using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Tutorial/New tutorial")]
public class TutorialScriptableObject : ScriptableObject
{
    public string title;
    public Image image;
    public string description;
}
