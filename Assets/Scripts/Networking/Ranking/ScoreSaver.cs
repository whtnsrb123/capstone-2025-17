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
        /*        if (GameStateManager.isServerTest && Managers.GameStateManager.IsClearGame())
                {
                    // Transmit clear info
                    SendScore();
                }
                else
                {
                    Debug.Log("Skip sending info");
                }
        */

        if (GameStateManager.isServerTest)
        {
            // Transmit clear info
            SendScore();
        }
        else
        {
            Debug.Log("Skip sending info");
        }
    }

    public async void SendScore()
    {
#if UNITY_EDITOR
        if (!PhotonNetwork.IsMasterClient) return;

        float clearTime = Managers.GameTimerManager.GetClearTime();
        string[] nicknames = GetRoomPlayerNicknames();

        await LeaderboardManager.Instance.SaveLeaderboardData(clearTime, nicknames);
#endif
    }

    public void Test_SendScore()
    {
        ShouldSendScore();
    }
}
