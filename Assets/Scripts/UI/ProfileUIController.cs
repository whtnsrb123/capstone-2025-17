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
        // lobby view �̺�Ʈ ���
        profileView.nicknameBtn.onClick.AddListener(() => OnClickNicknameBtn());
        profileView.nextBtn.onClick.AddListener(() => OnClickNextBtn());
        profileView.prevBtn.onClick.AddListener(() => OnClickPrevBtn());

        // lobby model �̺�Ʈ ��� 
        profileModel.NicknameUpdate += profileView.UpdateNicknameUI;
        profileModel.CharacterIdUpdate += profileView.UpdateCharacterUI;

        profileView.nicknameTMP.text = profileModel.Nickname;
    }

    private void OnDestroy()
    {
        // Destroy �� �̺�Ʈ ����
        profileView.nicknameBtn.onClick.RemoveListener(OnClickNicknameBtn);
        profileView.nextBtn.onClick.RemoveListener(OnClickNextBtn);
        profileView.prevBtn.onClick.RemoveListener(OnClickPrevBtn);

        profileModel.NicknameUpdate -= profileView.UpdateNicknameUI;
        profileModel.CharacterIdUpdate -= profileView.UpdateCharacterUI;
    }

    void OnClickNicknameBtn()
    {
        // �г��� ��ȿ�� �˻� 
        updatedNickname = profileView.nicknameInp.text;

        Debug.Log(updatedNickname);

        if (updatedNickname.Trim() == null)
        {
            profileView.nicknameTMP.text = "blank";
            Debug.Log("�г��� ���� ����");
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