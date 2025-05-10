using Photon.Pun;
using UnityEngine;

public class CameraFollow : MonoBehaviourPun
{
    public Transform cameraMount;

    void Awake()
    {
        Camera mainCam = Camera.main;
        if (GameStateManager.isServerTest && !photonView.IsMine)
            return;
        mainCam.transform.SetParent(cameraMount); // 카메라를 플레이어에 붙이고
        mainCam.transform.localPosition = Vector3.zero; // 위치 초기화
        mainCam.transform.localRotation = Quaternion.identity; // 회전 초기화
    }
}