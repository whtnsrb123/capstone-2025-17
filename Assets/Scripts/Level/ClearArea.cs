using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class ClearArea : MonoBehaviour
{
    [SerializeField]
    private int clearCount = 0;

    private BoxCollider collider;
    private LineRenderer line;

    void Start( )
    {
        collider = GetComponent<BoxCollider>();

        line = gameObject.AddComponent<LineRenderer>();
        line.positionCount = 5;
        line.loop = false;
        line.useWorldSpace = false;
        line.widthMultiplier = 0.05f;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = line.endColor = Color.cyan;

        DrawRectangle();
    }
    
    private void DrawRectangle()
    {
        Vector3 size = collider.size;
        Vector3 center = collider.center;

        float halfX = size.x / 2;
        float halfZ = size.z / 2;
        float fixedY = center.y - 0.4f;

        Vector3[] corners = new Vector3[5]
        {
            new Vector3(center.x - halfX, fixedY, center.z + halfZ),
            new Vector3(center.x + halfX, fixedY, center.z + halfZ),
            new Vector3(center.x + halfX, fixedY, center.z - halfZ),
            new Vector3(center.x - halfX, fixedY, center.z - halfZ),
            new Vector3(center.x - halfX, fixedY, center.z + halfZ)
        };

        line.SetPositions(corners);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
            clearCount++;

        if(PhotonNetwork.IsMasterClient)
        {
            if (clearCount >= PhotonNetwork.CurrentRoom.PlayerCount) // 방 인원 수로 고쳐야 함
            {
                int curMission = Managers.MissionManager.CurrentMission;
                Managers.MissionManager.CompleteMission(curMission);
                if (curMission < 3)
                {
                    Managers.MissionManager.GoNextMission();
                    Managers.GameStateManager.PlusTotalPlayTime(Managers.GameTimerManager.GetNowTime());
                }
                #region 정보 전송
                if (curMission == 3 || SceneManager.GetActiveScene().name == "Mission3")
                {
                    HandleGameEnding();
                }
                #endregion
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
            clearCount--;
    }

    private void HandleGameEnding()
    {
        Debug.Log($"HandleGameEnding() Excuted");

        Managers.GameStateManager.SetClearGame(true);

        if (PhotonNetwork.IsMasterClient)
        {
            ScoreSaver saver = FindAnyObjectByType<ScoreSaver>();
            saver.ShouldSendScore();
        }

        UIManager.Instance.GetUI(UIType.Ending).ShowUI();
        Time.timeScale = 0.25f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}