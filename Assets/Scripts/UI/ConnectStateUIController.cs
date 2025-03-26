using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ConnectStateUIController : MonoBehaviour
{
    public TextMeshProUGUI pingTMP;
    public TextMeshProUGUI connectStateTMP;
    public TextMeshProUGUI regionTMP;

    public void Start()
    {
        if(PhotonNetwork.IsConnected)
        {
            regionTMP.text = $"Server Region : {PhotonNetwork.CloudRegion}";
        }
        else
        {
            regionTMP.text = "Disconnected";
        }
    }

    private void Update()
    {
        pingTMP.text = $"now {PhotonNetwork.GetPing()}ms ";
        connectStateTMP.text = $"Connect Status : { (PhotonNetwork.IsConnected ? "Connecting" : "Disconnected")} ";
    }
}
