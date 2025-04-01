using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ClearArea : MonoBehaviour
{
    [SerializeField]
    private int clearCount = 0;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
            clearCount++;

        if(PhotonNetwork.IsMasterClient)
        {
            // if(clearCount > 4) // 방 인원 수로 고쳐야 함
            //    Managers.MissionManager.CompleteMission();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
            clearCount--;
    }
}