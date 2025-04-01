using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class MissionManager : MonoBehaviourPun ,IManager
{
    private Dictionary<int, bool> missionsStates = new Dictionary<int, bool>();
    
    int nextMission = 1; //다음 미션 번호
    
    public void Init()
    {
        Debug.Log("MissionManager 초기화 완료");
    }

    public void Clear()
    {
        for (int i = 0; i < missionsStates.Count; i++)
        {
            missionsStates[i] = false;
        }
        Debug.Log("MissionManager 클리어");
    }
    //특정 미션을 성공처리
    public void CompleteMission(int missionId)
    {
        photonView.RPC("MissionComplete", RpcTarget.All, missionId);
    }
    
    //모든 미션을 성공했는지 확인
    public bool AreAllMissionsComplete()
    {
        return missionsStates.Values.All(completed => completed); //(LINGQ), 람다표현식
    }
    
    //다음 미션으로 넘어가는 함수
    public void GoNextMission()
    {
        //첫 미션 시작이면 대기방에 4명이 전부 참여하고 있어야함.
        // if (nextMission == 1)
        // {
        //     Debug.Log(PhotonNetwork.CurrentRoom.MaxPlayers);
        //     //대기방에 4명이 전부 참여하고 있어야함.
        //      if (PhotonNetwork.CurrentRoom.PlayerCount != PhotonNetwork.CurrentRoom.MaxPlayers)
        //      {
        //          Debug.Log("The number of people has not been filled yet.");
        //          return;
        //      }
        // }
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogWarning("마스터 클라이언트가 아님, 씬 로드 취소");
            return;
        }

        Debug.Log("GoNextMission 호출됨");
        Managers.GameTimerManager.StartTimer(300f);

        foreach (var item in missionsStates.OrderBy(kv => kv.Key))
        {
            Debug.Log($"미션 상태 - ID: {item.Key}, 완료 여부: {item.Value}");
            if (!item.Value)
            {
                nextMission = item.Key;
                break;
            }
        }

        Debug.Log($"Mission{nextMission} 로딩 시작");
        PhotonNetwork.LoadLevel($"Mission{nextMission}");
        nextMission++;
    }

    #region RPC
    //특정 미션을 성공처리 RPC
    [PunRPC]
    public void MissionComplete(int missionId)
    {
        missionsStates[missionId] = true;
        
        //게임 종료조건 체크
        //GameStateManager.Instance.CheckGameEnd();
        Managers.GameStateManager.CheckGameEnd();
    }
    #endregion
}
