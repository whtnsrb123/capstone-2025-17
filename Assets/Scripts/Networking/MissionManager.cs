using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class MissionManager : MonoBehaviourPun
{
    public static MissionManager Instance {get; private set;}
    private Dictionary<int, bool> missionsStates = new Dictionary<int, bool>();
    
    int nextMission = 1; //다음 미션 번호

    
    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
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
        missionsStates.OrderBy(key => key.Key); 

        foreach (var item in missionsStates)
        {
            if (item.Value == false)
            {
                nextMission = item.Key;
                break;
            }
        }
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
        GameStateManager.Instance.CheckGameEnd();
    }
    #endregion
}
