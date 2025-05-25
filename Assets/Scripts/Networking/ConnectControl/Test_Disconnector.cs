using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Disconnector : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(SimulateDisconnected());
    }

    IEnumerator SimulateDisconnected()
    {
        Debug.Log("simulate disconnect");
        yield return new WaitForSeconds(7f);
        Debug.Log("try disconnect!");

        PhotonNetwork.Disconnect();
    }
}
