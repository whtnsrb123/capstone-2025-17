using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerInfo : MonoBehaviour
{
    public const int RequiredPlayerCount = 4;

    public const string PlayerActorNumbersKey = "PlayerActorNumbers";
    public const string ReadyStatesKey = "ReadyStates";
    public const string MatchTypeKey = "MatchType";
    public const string IsGameStartKey = "GameStart";

    public static ObservableArray<int> PlayerActorNumbers { get; private set; } = new ObservableArray<int>(4, -1);
    public static ObservableArray<bool> ReadyStates { get; private set; } = new ObservableArray<bool>(4, false);

    // 해시 키

    public enum RoomTypes
    {
        Random,
        Create,
        Join
    };

     int _maxPlayer;
    RoomTypes _roomType;

    public RoomTypes RoomType
    {
        get { return _roomType; }
        set { _roomType = value; }
    }

    public int MaxPlayer
    {
        get { return _maxPlayer; }
    }

    public static void InitServerInfo()
    {
        PlayerActorNumbers.EventOff = true;
        ReadyStates.EventOff = true;

        for (int i = 0; i < RequiredPlayerCount; i++)
        {
            PlayerActorNumbers[i] = -1;
            ReadyStates[i] = false;
        }
        PlayerActorNumbers.EventOff = false;
        ReadyStates.EventOff = false;
    }
}
