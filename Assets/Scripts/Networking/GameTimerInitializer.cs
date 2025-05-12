using UnityEngine;
using System.Collections;
using Photon.Pun;

public class GameTimerInitializer : MonoBehaviour
{
    [Tooltip("미션 제한 시간 (초 단위)")]
    public float missionDuration = 300f;

    private IEnumerator Start()
    {
        // 씬 안정화를 위해 약간의 딜레이 후 타이머 시작
        yield return new WaitForSeconds(0.1f);
        
        Managers.GameTimerManager.StartTimer(missionDuration);
        Debug.Log($"GameTimerInitializer: 타이머 {missionDuration}초 시작");
    }
}