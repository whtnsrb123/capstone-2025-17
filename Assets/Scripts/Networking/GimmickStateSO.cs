using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GimmickState", menuName = "Gimmick/GimmickState")]
public class GimmickStateSO : ScriptableObject
{
    public string gimmickId; // "Door_A", "Switch_3" 등 유니크 ID
    public bool isActivated;
}
