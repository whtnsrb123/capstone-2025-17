using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class MissionManager : MonoBehaviourPun
{
    public static MissionManager Instance {get; private set;}
    private Dictionary<int, bool> missionsStates = new Dictionary<int, bool>();
    
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
    
    //특정 미션을 성공처리 RPC
    [PunRPC]
    public void MissionComplete(int missionId)
    {
        missionsStates[missionId] = true;
        
        //게임 종료조건 체크
        GameStateManager.Instance.CheckGameEnd();
    }
    
    //모든 미션을 성공했는지 확인
    public bool AreAllMissionsComplete()
    {
        return missionsStates.Values.All(completed => completed); //(LINGQ), 람다표현식
    }
}
