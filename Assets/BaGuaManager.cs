using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaGuaManager : MonoBehaviour
{
    [SerializeField] GameObject BaGuaZhen;
    InputManager inputManager;
    public List<int> command_Temp = new List<int>();
    public int curPiviot;

    public Vector2 curPos;
    public GameObject realPiviot;

    [SerializeField] GameObject[] BaGuaText;
    [SerializeField] Transform spawnPos;
    [SerializeField] float spawnTimer;
    public bool isCommandActive;
    void Awake()
    {
        inputManager = GetComponent<InputManager>();
        curPos = realPiviot.transform.position;
    }

    void Update()
    {
        if (inputManager.baGua_Input && !isCommandActive)
        {
            BaGuaZhen.SetActive(true);
            realPiviot.transform.position = new Vector2(curPos.x + (inputManager.cameraInputX * 100), curPos.y + (inputManager.cameraInputY * 100));
            if (command_Temp.Count <= 3) 
            {
                if (inputManager.cameraInputX >= 0.99 && inputManager.cameraInputY >= -0.13 && inputManager.cameraInputY <= 0.13)
                {
                    BaGuaCommand(2);
                }
                else if (inputManager.cameraInputX <= -0.99 && inputManager.cameraInputY >= -0.13 && inputManager.cameraInputY <= 0.13)
                {
                    BaGuaCommand(6);
                }
                else if (inputManager.cameraInputY >= 0.99 && inputManager.cameraInputX >= -0.13 && inputManager.cameraInputX <= 0.13)
                {
                    BaGuaCommand(0);
                }
                else if (inputManager.cameraInputY <= -0.99 && inputManager.cameraInputX >= -0.13 && inputManager.cameraInputX <= 0.13)
                {
                    BaGuaCommand(4);
                }
                else if (inputManager.cameraInputX > 0.61 && inputManager.cameraInputX < 0.79 && inputManager.cameraInputY > 0.61 && inputManager.cameraInputX < 0.79)
                {
                    BaGuaCommand(1);
                }
                else if (inputManager.cameraInputX < -0.61 && inputManager.cameraInputX > -0.79 && inputManager.cameraInputY > 0.61 && inputManager.cameraInputX < 0.79)
                {
                    BaGuaCommand(7);
                }
                else if (inputManager.cameraInputX > 0.61 && inputManager.cameraInputX < 0.79 && inputManager.cameraInputY < -0.61 && inputManager.cameraInputX > -0.79)
                {
                    BaGuaCommand(3);
                }
                else if (inputManager.cameraInputX < -0.61 && inputManager.cameraInputX > -0.79 && inputManager.cameraInputY < -0.61 && inputManager.cameraInputX > -0.79)
                {
                    BaGuaCommand(5);
                }
            }

            if (inputManager.lockOn_Input)
            {
                command_Temp.Clear();
            }
        }
        else 
        {
            BaGuaZhen.SetActive(false);
            if (command_Temp.Count >= 2) 
            {
                isCommandActive = true;
            }
        }

        if (isCommandActive) 
        {
            CommandActive();
        }
    }
    private void FixedUpdate()
    {
        spawnTimer -= Time.fixedDeltaTime;
        if (spawnTimer <= 0) 
        {
            spawnTimer = 0;
        }
    }
    void BaGuaCommand(int index)
    {
        if (command_Temp.Count == 0)
        {
            command_Temp.Add(index);
        }
        else if(command_Temp.Count <= 3)
        {
            if (index != command_Temp[command_Temp.Count - 1])
            {
                command_Temp.Add(index);
            }
        }
    }
    void CommandActive()
    {
        if (command_Temp.Count != 0)
        {
            if (spawnTimer <= 0)
            {
                GameObject baguaText = Instantiate(BaGuaText[command_Temp[0]], new Vector3(spawnPos.position.x, spawnPos.position.y, spawnPos.position.z), Quaternion.identity);
                Destroy(baguaText, 1f);
                spawnTimer = 0.45f;
                command_Temp.Remove(command_Temp[0]);
            }
        }
        else 
        {
            isCommandActive = false;
        }
    }
}
