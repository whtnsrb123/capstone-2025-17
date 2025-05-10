using Photon.Pun;
using UnityEngine;

public class CameraFollow : MonoBehaviourPun
{
    public GameObject player;
    public Transform cameraMount; // 플레이어 자식으로 설정된 마운트 위치
    private Camera mainCam;

    void Awake()
    {
        player = gameObject;
        mainCam = Camera.main;
        
        if (GameStateManager.isServerTest && !photonView.IsMine)
            return;
        
        // 카메라를 cameraMount에 붙이고 초기화
        mainCam.transform.SetParent(cameraMount);
        mainCam.transform.localPosition = Vector3.zero;
        mainCam.transform.localRotation = Quaternion.identity;
        //mainCam.AddComponent<CameraMove>();
    }

    void LateUpdate()
    {
        if (GameStateManager.isServerTest && !photonView.IsMine) return;
        Vector3 dir = player.transform.position - mainCam.transform.position;
        

        Debug.DrawRay(mainCam.transform.position, dir, Color.blue);
        RaycastHit hit;

         if (Physics.Raycast(mainCam.transform.position, dir, out hit, dir.magnitude * 0.8f,
                 LayerMask.GetMask("Obstruction")))
         {
             mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, hit.point, 0.3f);
         }
         else
         {
             mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, cameraMount.transform.position, 0.3f);
         }
    }
}