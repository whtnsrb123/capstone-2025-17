using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class GeneratorForElevator : MonoBehaviourPun
{
    public Generator firstGenerator;
    public Generator secondGenerator;
    public Elevator elevator;
    public ShortCutSceneCamera cutSceneCamera;

    void Update()
    {
        if(firstGenerator.isOn && secondGenerator.isOn)
        {
            photonView.RPC(nameof(OnElevator), RpcTarget.All);
        }
    }
    
    [PunRPC]
    void OnElevator()
    {
        cutSceneCamera.PlayShortCutScene();


        firstGenerator.FinishGenerate();
        secondGenerator.FinishGenerate();

        elevator.gameObject.tag = "Interactable";
        elevator.Interact();

            
        Destroy(this);
    }
}