using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Elevator : MonoBehaviourPun, IInteractable
{
    public bool isMoving = false;
    public bool isDown = true;

    public Vector3 downPosition;
    public Vector3 upPosition;
    
    void Start()
    {
        
    }

    public void Interact()
    {
        if (isMoving) return;

        isMoving = true;
        StartCoroutine(MoveElevator());
        
        //일단 급한대로 이렇게.. 엘베 내려와 있으면 RPC로 엘베 올라감
        if (GameStateManager.isServerTest && isDown && PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(nameof(UpElevatorRPC), RpcTarget.All);
        }
    }

    [PunRPC]
    void UpElevatorRPC()
    {
        StartCoroutine(MoveElevator());
    }
    
    IEnumerator MoveElevator()
    {
        Vector3 targetPosition = isDown ? upPosition : downPosition;
        Vector3 startPosition = transform.position;
        float time = 0;
        float duration = 5f;
        
        while (time < duration)
        {
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            yield return null;
        }

        isDown = !isDown;
        isMoving = false;
    }
}