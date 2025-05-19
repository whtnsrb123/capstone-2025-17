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
        photonView.RPC(nameof(InteractWithElevator), RpcTarget.All);
    }

    [PunRPC]
    public void InteractWithElevator()
    {
        if (isMoving) return;

        isMoving = true;
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