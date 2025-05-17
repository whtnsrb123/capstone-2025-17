using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class BatteryBox : MonoBehaviourPun, IInteractable
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
        PhotonView view = player.GetComponent<PhotonView>();
        photonView.RPC(nameof(InteractRPC), RpcTarget.All, view.ViewID);
    }
    
    [PunRPC]
    public void InteractRPC(int viewID)
    {
        Debug.Log("InteractRPC 호출됨!");

        PhotonView view = PhotonView.Find(viewID);
        if (view == null)
        {
            Debug.LogError($"[BatteryBox] viewID {viewID}에 해당하는 PhotonView를 찾을 수 없습니다.");
            return;
        }

        GameObject player = view.gameObject;
        PickUpController pickUpController = player.GetComponent<PickUpController>();
        GameObject pickUp = player.transform.Find("pickPosition").GetChild(0).gameObject;

        if (pickUp == null)
        {
            Debug.LogError($"[BatteryBox] PickUpController가 {player.name}에 존재하지 않습니다.");
            return;
        }

        Battery battery = pickUp.GetComponent<Battery>();
        if (battery == null)
        {
            Debug.LogWarning($"[BatteryBox] heldObject '{pickUp.name}'에 Battery 컴포넌트가 없습니다.");
            return;
        }

        pickUpController.DropObject();
        pickUp.tag = "Untagged";
        Rigidbody rb = pickUp.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        pickUp.transform.position = batteryPositions[batteryCount].position;
        pickUp.transform.eulerAngles = new Vector3(-90f, 90f, 0f);

        lights[batteryCount].GetComponent<MeshRenderer>().material = greenLightMaterial;
        lights[batteryCount].GetComponent<Light>().color = greenLightColor;

        batteryCount++;
        Debug.Log($"batteryCount++! : {batteryCount}");

        if (batteryCount == 4)
        {
            door.OpenDoor();
            cutScene.PlayShortCutScene();
        }
    }

    public void Interact()
    {

    }
}