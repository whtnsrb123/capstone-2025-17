using System;
using UnityEngine;

public class ClientInfo : MonoBehaviour
{
    string nickname;
    int characterId;

    public Action<string> NicknameUpdate;
    public  Action<int> CharacterIdUpdate;


    public string Nickname
    {
        get 
        {
            nickname = PlayerPrefs.GetString("Nickname", DefaultNickname());
            return nickname;
        }
        set 
        {
            nickname = value;
            NicknameUpdate?.Invoke(nickname);
            PlayerPrefs.SetString("Nickname", value);
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
        string defaultNickname = $"USER_{UnityEngine.Random.Range(1000, 9999)}";
        return defaultNickname;
    }
}
