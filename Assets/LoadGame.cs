using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGame : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void Loadscene(string scene) 
    {
        SceneManager.LoadScene(scene);
    }
}
