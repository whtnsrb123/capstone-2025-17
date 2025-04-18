using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

// View 
public class ProfileUI : MonoBehaviour
{
    [SerializeField]
    GameObject characterRender;
    [SerializeField]
    MaterialStorage materials;

    SkinnedMeshRenderer sm_renderer;

    // 매치메이킹 버튼
    public Button randomBtn;
    public Button createBtn;
    public Button joinBtn;

    // 프로필 관련 버튼
    public Button optionBtn;
    public Button nicknameBtn;
    public Button nextBtn;
    public Button prevBtn;

    public TMP_InputField nicknameInp;
    public TextMeshProUGUI nicknameTMP;
    public TextMeshProUGUI nicknameGuideTMP;

    private void Awake()
    {
        sm_renderer = characterRender.GetComponent<SkinnedMeshRenderer>();
    }

    // M 변경 -> V 업데이트
    public void UpdateCharacterUI(int characterId)
    {
        sm_renderer.material = materials.GetMesh(characterId);
    }

    public void UpdateNicknameUI(string nickname)
    {
        nicknameTMP.text = nickname;
    }
}
