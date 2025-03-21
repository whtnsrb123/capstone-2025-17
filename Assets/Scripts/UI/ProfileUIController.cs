using UnityEngine;

public class ProfileUIController : MonoBehaviour
{    
    ClientInfo profileModel;
    ProfileUI profileView;

    const int characterCount = 12;

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

    void OnClickNicknameBtn()
    {
        // 닉네임 유효성 검사 
        updatedNickname = profileView.nicknameInp.text;

        Debug.Log(updatedNickname);

        if (updatedNickname.Trim() == null)
        {
            profileView.nicknameTMP.text = "blank";
            Debug.Log("닉네임 공백 상태");
        }

        profileModel.Nickname = updatedNickname;

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