using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionSceneInitializer : MonoBehaviour
{
    void Start()
    {
        // 씬 로드 후 최초 한 번만 호출됨
        Debug.Log(Managers.Instance); // 접근만 해도 Init()이 내부에서 호출됨
    }
}
