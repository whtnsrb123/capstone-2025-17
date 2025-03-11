using System;
using UnityEngine;

public class ClientInfo : MonoBehaviour
{
    string nickname;
    int characterId;

    // ProfileUIController 에서 이벤트를 등록하여, 데이터 변경 시 UI가 업데이트 되게 한다
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
        // 닉네임 설정 전, 기본 닉네임을 생성한다 
        string defaultNickname = $"USER_{UnityEngine.Random.Range(1000, 9999)}";
        return defaultNickname;
    }
}
