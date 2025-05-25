using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuggingNetworkVariables : MonoBehaviour
{
    [ShowInInspector, ReadOnly] private ConnectState d_CurrentState;
    [ShowInInspector, ReadOnly] private ConnectState d_ClientState;
    void Update()
    {
         d_CurrentState = ClientInfo.sCurrentState;
         d_ClientState = ClientInfo.sClientState;
    }
}
