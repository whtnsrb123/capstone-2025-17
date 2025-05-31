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
        _ = LoadRanking();
    }

    public async Task LoadRanking()
    {
        List<Dictionary<string, object>> topFive =  await LeaderboardManager.Instance.GetAllLeaderboardData();

        int index = 0;
        foreach (var row in topFive)
        {
            float time = Convert.ToSingle(row["time"]);
            string[] players = ((List<object>)row["players"]).Select(p => p.ToString()).ToArray();
            
            infoTMPs[index].alignment = TextAlignmentOptions.Left;
            infoTMPs[index++].text = $"{SetScoreFormat(time)}\t\t{string.Join(", ", players)}";

        }

        loadingImage.SetActive(false);
        // await LeaderboardManager.Instance.DeleteAllLeaderboardData();

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

    private string SetScoreFormat(float time)
    {
        int min = (int) (time / 60f);
        int sec = (int)(time % 60f);

        string totalStr = string.Empty;

        if (min > 0 )
        {
            totalStr = $"{min}분 {sec}초";
        }
        else if (min == 0)
        {
            totalStr = $"{sec}초\t";
        }
        return totalStr;
    }
}
