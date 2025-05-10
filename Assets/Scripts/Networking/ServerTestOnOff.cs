using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ServerTestOnOff : MonoBehaviour
{
    private TextMeshProUGUI tmp;
    // Start is called before the first frame update
    void Start()
    {
        tmp = gameObject.transform.Find("ServerState").GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    //멀티/1인 on/off버튼
    public void ServerTestChange()
    {
        GameStateManager.isServerTest = !GameStateManager.isServerTest;

        if (GameStateManager.isServerTest)
        {
            tmp.text = "Current State : Multi";
        }
        else
        {
            tmp.text = "Current State : Single";
        }
    }
}
