using Photon.Pun;
using UnityEngine;

public class ScoreSaver : MonoBehaviour
{
    public string[] GetRoomPlayerNicknames()
    {
        var players = PhotonNetwork.CurrentRoom.Players;
        string[] nicknames = new string[PhotonNetwork.CurrentRoom.PlayerCount];

        Debug.Assert(players.Count == nicknames.Length, "wrong player count info!!");


        int index = 0;
        foreach (var p in players)
        {
            string nickname = p.Value.CustomProperties[ClientInfo.NicknameKey].ToString().Trim();
            nicknames[index++] = nickname;
            Debug.Log(nickname);
        }
        return nicknames;
    }

    public void ShouldSendScore()
    {
        if (GameStateManager.isServerTest && Managers.GameStateManager.IsClearGame())
        {
            // Transmit clear info
            SendScore();
        }
        else
        {
            Debug.Log("[ Clear Condition ] : Skip sending info");
        }

    }

    public async void SendScore()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        float clearTime = Managers.GameStateManager.GetTotalPlayTime();
        Debug.Log($"[ Clear Condition ] : 클리어 시간 : {clearTime }");
        string[] nicknames = GetRoomPlayerNicknames();

        Debug.Log("클리어 정보  전송 준비");

        await LeaderboardManager.Instance.SaveLeaderboardData(clearTime, nicknames);
    }
}
