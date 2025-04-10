using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryBox : MonoBehaviour, IInteractable
{
    public int batteryCount = 0;

    public Transform[] batteryPositions;
    public GameObject[] lights;

    public Material greenLightMaterial;
    public Color greenLightColor;

    public Autodoor door;
    public ShortCutSceneCamera cutScene;

    void Start()
    {
        
    }

    public void Interact()
    {

        // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        // 손에 들고 있는 배터리 오브젝트를 강제로 옮기는 과정이 필요함
        // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        lights[batteryCount].GetComponent<MeshRenderer>().material = greenLightMaterial;
        lights[batteryCount].GetComponent<Light>().color = greenLightColor;
        batteryCount++;

        if(batteryCount == 4)
        {
            door.OpenDoor();
            cutScene.PlayShortCutScene();
        }
    }
}