using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivePanelUI : MonoBehaviour
{

    public static  void Active(GameObject panel)
    {
        panel.SetActive(true);

    }

    public static void Inactive(GameObject inactivePanel)
    {
        inactivePanel.SetActive(false);
        inactivePanel = null;
    }

}
