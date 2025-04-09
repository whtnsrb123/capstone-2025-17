using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerInfo : MonoBehaviour
{
    public const int RequiredPlayerCount = 4;

    // 해시 키
    public const string PlayerActorNumbers = "PlayerActorNumbers";

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

}
