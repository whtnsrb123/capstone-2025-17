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
            regionTMP.text = $"연결된 국가 : {PhotonNetwork.CloudRegion}";
        }
        else
        {
            regionTMP.text = "접속 끊김";
        }
    }

    private void Update()
    {
        pingTMP.text = $"Ping :  {PhotonNetwork.GetPing()}ms ";
        connectStateTMP.text = $"연결 상태 : { (PhotonNetwork.IsConnected ? "연결됨" : "연결 끊김")} ";
    }
}
