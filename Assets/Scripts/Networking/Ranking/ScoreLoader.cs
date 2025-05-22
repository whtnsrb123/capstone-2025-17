using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine.UI;

public class ScoreLoader : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI[] infoTMPs;
    [SerializeField] GameObject loadingImage;

    PopupUI popupUI;

    private void Start()
    {
        popupUI = GetComponent<PopupUI>();
    }

    public void InvokeLoadRanking()
    {
#if UNITY_EDITOR
        _ = LoadRanking();
#endif
    }

    public async Task LoadRanking()
    {
        List<Dictionary<string, object>> topFive =  await LeaderboardManager.Instance.GetAllLeaderboardData();

        int index = 0;
        foreach (var row in topFive)
        {
            float time = Convert.ToSingle(row["time"]);
            string[] players = ((List<object>)row["players"]).Select(p => p.ToString()).ToArray();

            infoTMPs[index++].text = $"{time:F2}초\t\t{string.Join(", ", players)}";

            loadingImage.SetActive(false);
        }


    }

    public void OnClickRankingboard()
    {
        loadingImage.SetActive(true);
        popupUI.ShowUI();
    }

    public void OnClickCloseBtn()
    {
        popupUI.HideUI();
    }
}
