using Photon.Pun;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomUI : MonoBehaviour
{

    #region Room UI 

    [Header("Random UI")]
    [TabGroup("Buttons")] public Button randomBtn;

    [Header("Room UI")]
    [TabGroup("Buttons")] public Button readyOrStartBtn;
    [TabGroup("Buttons")] public Button leaveBtn;
    [TabGroup("Buttons")] public TextMeshProUGUI roomCode;

    [TabGroup("Character Objects")] public GameObject[] playersUI;
    [TabGroup("Nicknames")] public TextMeshProUGUI[] nicknamesUI;
    [TabGroup("RawImage")] public GameObject[] playersRawImage;
    [TabGroup("Ready")] public TextMeshProUGUI[] playersReadyStatesUI;
    [TabGroup("Crown")] public Image[] masterClientCrown;
    
    // create panel
    [Header("Create Room")]
     public Button c_confirmBtn;
    public Button c_cancelBtn;

    // joine panel

    [Header("Join Room")]
    public TMP_InputField roomCodeTMPInp;
    public Button j_confirmBtn;
    public Button j_cancelBtn;
    #endregion


    // 캐릭터 모델의 메시가 저장된 Scriptable Object 변수 
    [SerializeField] MaterialStorage storage;
    SkinnedMeshRenderer[] smRenderers;
    TMP_Text readyOrStartButtonTMP;

    private void Start()
    {
        SetSkinnedMeshRenderers();
        readyOrStartButtonTMP = readyOrStartBtn.GetComponentInChildren<TMP_Text>();
    }

    void SetSkinnedMeshRenderers()
    {
        smRenderers = new SkinnedMeshRenderer[4];
        for (int i = 0; i < 4; i++)
        {
            smRenderers[i] = playersUI[i].GetComponentInChildren<SkinnedMeshRenderer>();
        }
    }

    public void UpdatePlayer(int index, int actorNumber)
    {
        if (actorNumber == -1)
        {
            // 플레이어 퇴장
            playersUI[index].SetActive(false);
            nicknamesUI[index].text = string.Empty;
            playersRawImage[index].SetActive(false);
            playersReadyStatesUI[index].text = string.Empty;
        }
        else
        {
            int characterId = (int)PhotonNetwork.CurrentRoom.Players[actorNumber].CustomProperties[ClientInfo.CharacterIdKey];
            string nickname = (string)PhotonNetwork.CurrentRoom.Players[actorNumber].CustomProperties[ClientInfo.NicknameKey];

            playersUI[index].SetActive(true);
            playersRawImage[index].SetActive(true);
            playersReadyStatesUI[index].text = ServerInfo.ReadyStates[index] ? "준비 완료" : "준비하기";

            smRenderers[index].material = storage.GetMesh(characterId);
            nicknamesUI[index].text = nickname;
        }
    }

    public void UpdateReadyState(int index, bool ready)
    {
        playersReadyStatesUI[index].text = ServerInfo.ReadyStates[index] ? "준비 완료" : "준비하기";
        if (ServerInfo.PlayerActorNumbers[index] == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            readyOrStartBtn.GetComponentInChildren<TMP_Text>().text = ServerInfo.ReadyStates[index] ? "준비 완료" : "준비하기";
        }
    }

    public void InitPanel()
    {
        for (int i = 0; i < ServerInfo.PlayerActorNumbers.Length; i++)
        {
            UpdatePlayer(i, ServerInfo.PlayerActorNumbers[i]);
        }
    }

}
