using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 생성되는 프리팹의 컴포넌트로 부착되어 있어야 실행된다 
public class PlayerSkinApplier : MonoBehaviour, IPunInstantiateMagicCallback
{
    [SerializeField] MaterialStorage storage;

    SkinnedMeshRenderer sm;

    // 객체가 생성될 때 실행된다 
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        int skinId = 0, skinInfoIndex = 0;

        object[] data = info.photonView.InstantiationData;

        if (data != null && data.Length > 0)
        {
            skinId = (int)data[skinInfoIndex];
        }

        ApplySkin(skinId);
    }

    public void ApplySkin(int skinId)
    {
        sm = this.gameObject.GetComponentInChildren<SkinnedMeshRenderer>();

        Debug.Assert( sm != null, "skin mesh renderer is null!" );

        sm.material = storage.GetMesh(skinId);

    }
}
