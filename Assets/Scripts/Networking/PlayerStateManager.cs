using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStateManager : MonoBehaviourPun
{
    public static PlayerStateManager Instance { get;  private set;  }
    public enum PlayerState
    {
        //정상
        Normal,
        //물에 젖음
        Wet,
        //TODO: 젖음 외에 다른 상태 추가
    }
    Dictionary<int, PlayerState> playerStates = new Dictionary<int, PlayerState>(); //플레이어 상태를 관리하는 딕셔너리 <playerId, PlayerState>
    Dictionary<int, bool> playerEscaped = new Dictionary<int, bool>();//플레이어 도착지점 도달 여부를 관리하는 딕셔너리 <playerId, bool>

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 변경되어도 유지
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    //플레이어의 상태를 업데이트 (ex.정상 -> 젖음, 젖음 -> 정상)
    public void UpdatePlayerState(int playerId, PlayerState state)
    {
        if (!playerStates.ContainsKey(playerId))
        {
            playerStates.Add(playerId, state);
        }
        
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(nameof(RpcUpdatePlayerState), RpcTarget.All, playerId, (int)state); //enum대신 int로 변환하면 데이터 패킷 크기를 줄일 수 있다.
        }
        else //방장이 아니라면 상태변화 요청
        {
            photonView.RPC(nameof(RequestUpdatePlayerState), RpcTarget.MasterClient, playerId, (int)state);
        }
    }

    [PunRPC]
    private void RpcUpdatePlayerState(int playerId, int state)
    {
        playerStates[playerId] = (PlayerState) state;
    }
    [PunRPC]
    private void RequestUpdatePlayerState(int playerId, int state)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // 마스터 클라이언트가 변경된 상태를 모든 클라이언트에 전파
            photonView.RPC(nameof(RpcUpdatePlayerState), RpcTarget.All, playerId, state);
        }
    }
    
    //플레이어의 탈출여부를 업데이트 (ex. n번 플레이어가 도착지점에 도달 시 호출)
    public void UpdatePlayerEscaped(int playerId, bool escaped)
    {
        if (!playerEscaped.ContainsKey(playerId))
        {
            playerEscaped.Add(playerId, escaped);
        }
        //마스터 클라이언트가 플레이어 탈출 여부를 동기화
        //마스터 클라이언트만 도착 여부를 관리하도록 하고, RpcUpdatePlayerEscaped로 모든 클라이언트에 전파해야 함.
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(nameof(RpcUpdatePlayerEscaped), RpcTarget.All, playerId, escaped);
        }
        else
        {
            photonView.RPC(nameof(RequestUpdatePlayerEscaped), RpcTarget.MasterClient, playerId, escaped);
        }
    }
    [PunRPC]
    private void RpcUpdatePlayerEscaped(int playerId, bool escaped)
    {
        playerEscaped[playerId] = escaped;
    }
    [PunRPC]
    private void RequestUpdatePlayerEscaped(int playerId, bool escaped)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(nameof(RpcUpdatePlayerEscaped), RpcTarget.All, playerId, escaped);
        }
    }
    
    //특정 플레이어가 도착지점에 도달했는지 체크하는 함수
    public bool CheckPlayerEscape(int playerId)
    {
        return playerEscaped.TryGetValue(playerId, out bool escaped) && escaped;
    }
}
