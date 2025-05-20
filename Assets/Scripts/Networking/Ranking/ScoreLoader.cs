using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using System.Linq;
using JetBrains.Annotations;

public class ScoreLoader : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI[] infoTMPs;

    public void InvokeLoadRanking()
    {
#if UNITY_EDITOR
        _ = LoadRanking();
#endif
    }

    public async Task LoadRanking()
    {
        List<Dictionary<string, object>> topFive =  await LeaderboardManager.Instance.GetAllLeaderboardData();

        Debug.Log("================================================");

        int index = 0;
        foreach (var row in topFive)
        {
            float time = Convert.ToSingle(row["time"]);
            string[] players = ((List<object>)row["players"]).Select(p => p.ToString()).ToArray();

            infoTMPs[index++].text = $"{time:F2}초\t\t{string.Join(", ", players)}";
        }


    }






}
