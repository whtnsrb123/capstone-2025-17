using UnityEngine;
using UnityEngine.UI;
public class MuteController : MonoBehaviour
{
    public Sprite on;
    public Sprite off;

    [SerializeField] private Toggle masterMute;
    [SerializeField] private Toggle BGMMute;
    [SerializeField] private Toggle SFXMute;
    [SerializeField] private Toggle chatMute;
    [SerializeField] private Toggle voiceMute;

    [SerializeField] private Image masterSprite;
    [SerializeField] private Image BGMSprite;
    [SerializeField] private Image SFXSprite;
    [SerializeField] private Image chatSprite;
    [SerializeField] private Image voiceSprite;

    private string masterMuteStr = "MasterMute";
    private string BGMMuteStr = "BGMMute";
    private string SFXMuteStr = "SFXMute";
    private string chatMuteStr = "ChatMute";
    private string voiceMuteStr = "VoiceMute";

    private void Start()
    {
        ConnectUI();
        LoadMute();
    }

    private void ConnectUI()
    {
        masterMute.onValueChanged.AddListener(SetMasterMute);
        BGMMute.onValueChanged.AddListener(SetBGMMute);
        SFXMute.onValueChanged.AddListener(SetSFXMute);
        chatMute.onValueChanged.AddListener(SetChatMute);
        voiceMute.onValueChanged.AddListener(SetVoiceMute);
    }

    private void LoadMute()
    {
        bool isOn;

        // Mute 불러오기
        if (PlayerPrefs.HasKey(masterMuteStr))
        {
             isOn = PlayerPrefs.GetInt(masterMuteStr) == 1;
        }
        else
        {
            isOn = false;
        }
        masterSprite.sprite = (isOn ? on : off);


        if (PlayerPrefs.HasKey(BGMMuteStr))
        {
            isOn = PlayerPrefs.GetFloat(BGMMuteStr) == 1;
        }
        else
        {
            isOn = false;
        }
        BGMSprite.sprite = (isOn ? on : off);

        if (PlayerPrefs.HasKey(SFXMuteStr))
        {
            isOn = PlayerPrefs.GetFloat(SFXMuteStr) == 1;
        }
        else
        {
            isOn = false;
        }
        SFXSprite.sprite = (isOn ? on : off);

        if (PlayerPrefs.HasKey(chatMuteStr))
        {
            isOn = PlayerPrefs.GetFloat(chatMuteStr) == 1;
        }
        else
        {
            isOn = false;
        }
        chatSprite.sprite = (isOn ? on : off);

        if (PlayerPrefs.HasKey(voiceMuteStr))
        {
            isOn = PlayerPrefs.GetFloat(voiceMuteStr) == 1;
        }
        else
        {
            isOn = false;
        }
        voiceSprite.sprite = (isOn ? on : off);

    }

    public void SetMasterMute(bool isOn)
    {
        masterSprite.sprite = (isOn ? on : off);
    }

    public void SetBGMMute(bool isOn)
    {
        BGMSprite.sprite = (isOn? on : off);
    }

    public void SetSFXMute(bool isOn)
    {
        SFXSprite.sprite  =(isOn? on : off);
    }

    public void SetChatMute(bool isOn)
    {
        chatSprite.sprite =(isOn? on : off);
    }

    public void SetVoiceMute(bool isOn)
    {
        voiceSprite.sprite =(isOn? on : off);
        if (ChatManager.Instance != null)
        {
            ChatManager.Instance.SetTransmitEnabled(!isOn);
        }
        else
        {
            Debug.Log("Chat manager is null!");
        }
    }
}
