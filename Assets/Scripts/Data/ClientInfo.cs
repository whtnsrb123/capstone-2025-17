using System;
using UnityEngine;
public enum ConnectState
{
    Idle,
    Lobby,
    Room,
    InGame,
    Disconnected
}
public class ClientInfo : MonoBehaviour
{
    public static ConnectState sCurrentState = ConnectState.Idle;
    public static ConnectState sClientState = ConnectState.Idle;

    public const string NicknameKey = "NicknameKey";
    public const string CharacterIdKey = "CharacterIdKey";

    string nickname;
    int characterId;

    // ProfileUIController 에서 이벤트를 등록하여, 데이터 변경 시 UI가 업데이트 되게 한다
    public Action<string> NicknameUpdate;
    public  Action<int> CharacterIdUpdate;


    public string Nickname
    {
        get 
        {
            nickname = PlayerPrefs.GetString(NicknameKey, DefaultNickname());
            return nickname;
        }
        set 
        {
            nickname = value;
            NicknameUpdate?.Invoke(nickname);
            PlayerPrefs.SetString(NicknameKey, value);
        }
    }

    public int CharacterId
    {
        get
        {
            return characterId;
        }
        set
        {
            characterId = value;
            CharacterIdUpdate?.Invoke(characterId);
        }
    }

    string DefaultNickname()
    {
        // 닉네임 설정 전, 기본 닉네임을 생성한다 
        string defaultNickname = $"USER_{UnityEngine.Random.Range(1000, 9999)}";
        return defaultNickname;
    }
}
