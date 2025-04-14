using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class GoalArea : MonoBehaviourPun
{
    private HashSet<int> goaledPlayers = new HashSet<int>();
    private const int requiredPlayerCount = PhotonNetwork.CurrentRoom.PlayerCount;
    // Start is called before the first frame update

    private void OnTriggerEnter(Collider other)
    {
        PhotonView view = other.GetComponent<PhotonView>();
        if (view != null && view.IsMine && view.CompareTag("Player"))
        {
            photonView.RPC(nameof(RegisterPlayerGoal), RpcTarget.MasterClient, view.ViewID);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PhotonView view = other.GetComponent<PhotonView>();
        if (view != null && view.IsMine && view.CompareTag("Player"))
        {
            photonView.RPC(nameof(UnRegisterPlayerGoal), RpcTarget.MasterClient, view.ViewID);
        }
    }

    [PunRPC]
    void RegisterPlayerGoal(int viewId)
    {
        goaledPlayers.Add(viewId);
        Debug.Log($"현재 도착한 인원 수 : {goaledPlayers.Count}");

        if (goaledPlayers.Count >= requiredPlayerCount)
        {
            int cur = Managers.MissionManager.CurrentMission;
            //지금 미션 클리어 처리
            Managers.MissionManager.CompleteMission(cur); //CheckGameEnd까지 확인함
            
            //다음 미션으로 씬 로드
            Managers.MissionManager.GoNextMission();
        }
    }

    [PunRPC]
    void UnRegisterPlayerGoal(int viewId)
    {
        goaledPlayers.Remove(viewId);
        Debug.Log($"[이탈] 현재 도착한 인원 수 : {goaledPlayers.Count}");
    }
}
