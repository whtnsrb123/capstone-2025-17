using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerInfo : MonoBehaviour
{
    public const int CMaxPlayer = 4;
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
        set { _maxPlayer = value; }
    }

}
