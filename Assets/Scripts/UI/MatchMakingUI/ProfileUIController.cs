using System.Text.RegularExpressions;
using UnityEngine;

public class ProfileUIController : MonoBehaviour
{    
    ClientInfo profileModel;
    ProfileUI profileView;

    const int characterCount = 12; // 플레이어가 선택할 수 있는 캐릭터의 수
    const int maxNicknameLength = 10; // 닉네임 최대 길이
    Regex nicknameRegex = new Regex("^[가-힣a-zA-Z0-9_-]+$"); // 닉네임 입력 범위 

    string updatedNickname;

    void Awake()
    {
        profileModel = GetComponent<ClientInfo>();
        profileView = GetComponent<ProfileUI>();
    }

    private void Start()
    {
        // lobby view 이벤트 등록
        profileView.nicknameBtn.onClick.AddListener(OnClickNicknameBtn);
        profileView.nextBtn.onClick.AddListener(OnClickNextBtn);
        profileView.prevBtn.onClick.AddListener(OnClickPrevBtn);

        // lobby model 이벤트 등록 
        profileModel.NicknameUpdate += profileView.UpdateNicknameUI;
        profileModel.CharacterIdUpdate += profileView.UpdateCharacterUI;

        profileView.nicknameTMP.text = profileModel.Nickname;
    }

    private void OnDestroy()
    {
        // Destroy 시 이벤트 해제
        profileView.nicknameBtn.onClick.RemoveListener(OnClickNicknameBtn);
        profileView.nextBtn.onClick.RemoveListener(OnClickNextBtn);
        profileView.prevBtn.onClick.RemoveListener(OnClickPrevBtn);

        profileModel.NicknameUpdate -= profileView.UpdateNicknameUI;
        profileModel.CharacterIdUpdate -= profileView.UpdateCharacterUI;
    }

    // 닉네임 유효성 검사 
    void OnClickNicknameBtn()
    {
        // 닉네임 변경 요청에 대한 안내 메시지 
        string guideToNickname = string.Empty;

        updatedNickname = profileView.nicknameInp.text;

        Debug.Log(updatedNickname);

        if (string.IsNullOrWhiteSpace(updatedNickname))
        {
            guideToNickname = "닉네임이 공백입니다.";
            Debug.Log("닉네임 공백 상태");
        }
        else if (updatedNickname.Length >= maxNicknameLength)
        {
            guideToNickname = $"최대 {maxNicknameLength} 글자 입력 가능합니다.";
            Debug.Log("너무 길어");
        }
        else if (!nicknameRegex.IsMatch(updatedNickname))
        {
            guideToNickname = "한글, 영어, 0-9, -만 입력 가능합니다.";
            Debug.Log("이상한 문자들도 사용함");
        }
        else
        {
            // 정상적인 닉네임이 입력된 경우
            guideToNickname = "수정됐습니다.";
            profileModel.Nickname = updatedNickname;
        }

        profileView.nicknameGuideTMP.text = guideToNickname;

    }
    void OnClickNextBtn()
    {
        profileModel.CharacterId = (profileModel.CharacterId + 1) % characterCount;
    }

    void OnClickPrevBtn()
    {
        profileModel.CharacterId = (profileModel.CharacterId - 1 + characterCount) % characterCount;
    }

    public void OnClickCharacterBtn(int id)
    {
        profileModel.CharacterId = id;
    }

}