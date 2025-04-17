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

    public void Interact(GameObject player)
    {
        if(player.GetComponent<PickUpController>().heldObject.GetComponent<Battery>() == null)
            return;
        
        GameObject targetBattery = player.GetComponent<PickUpController>().heldObject;
        player.GetComponent<PickUpController>().DropObject();
        targetBattery.tag = "Untagged";
        targetBattery.GetComponent<Rigidbody>().useGravity = false;
        targetBattery.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        targetBattery.transform.position = batteryPositions[batteryCount].position;
        targetBattery.transform.eulerAngles = new Vector3(-90f, 90f, 0f);

        lights[batteryCount].GetComponent<MeshRenderer>().material = greenLightMaterial;
        lights[batteryCount].GetComponent<Light>().color = greenLightColor;
        batteryCount++;

        if(batteryCount == 4)
        {
            door.OpenDoor();
            cutScene.PlayShortCutScene();
        }
    }

    public void Interact()
    {

    }
}