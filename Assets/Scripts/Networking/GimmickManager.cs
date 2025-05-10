using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class GimmickManager : MonoBehaviourPun, IManager
{
    public List<GimmickStateSO> gimmickStates;

    public void Init()
    {
        gimmickStates = new List<GimmickStateSO>(
            Resources.LoadAll<GimmickStateSO>("Gimmicks")
        );
        Debug.Log($"GimmickManager 초기화 완료 - {gimmickStates.Count}개 로드됨");
    }

    public void Clear()
    {
        Debug.Log("GimmickManager 클리어");
    }
    public void ActivateGimmick(string gimmickId)
    {
        photonView.RPC(nameof(ActivateGimmickRPC), RpcTarget.All, gimmickId);
    }

    [PunRPC]
    public void ActivateGimmickRPC(string gimmickId)
    {
        var gimmick = gimmickStates.Find(g => g.gimmickId == gimmickId);
        if (gimmick != null)
        {
            gimmick.isActivated = true;
            Debug.Log($"{gimmickId} 기믹이 활성화됨");
        }
    }
    
    public bool IsGimmickActivated(string gimmickId)
    {
        var gimmick = Managers.GimmickManager.gimmickStates.Find(g => g.gimmickId == gimmickId); 
        //gimmickStates.Find(g => g.gimmickId == gimmickId);
        return gimmick != null && gimmick.isActivated;
    }
}
