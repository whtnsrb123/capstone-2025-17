using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UI;

/// <summary>
/// 2025.03.03 KimYuJin
/// Slider의 min Value는 0.001f로 사용해야 한다. 
/// Sound(3D 포함) 재생 및 관리를 담당하는 클래스 
/// 
/// 3D Sound가 필요한 Sound - 캐릭터가 내는 효과음, NPC나 물체의 소리 효과음 등은 
/// 다르게 재생해야함. -> AudioSourceInstance.PlayClipAtPoint(clip, Vector3)을 사용하면 재생할 수 있음
/// 근데 이 함수의 동작 원리가 객체를 생성하고 재생 끝나면 소멸하는 거 같아서 
/// 직접 audiosource pool을 만들어서 관리하는 게 나을 수도 있을 거 같다는 생각이 든다
/// 
/// SoundManager를 사용하는 이유 중 하나는 효과음 같은 경우 오브젝트에서 
/// 재생하면 오브젝트가 사라질 때 소리가 끊기기 때문
/// 그리고 Sound Source와 Clip을 한 군데에서 관리해서 편하다는 장점이 있다.
/// 
/// Audio Source는 Sound를 재생해주는 Unity의 컴포넌트이다.
/// AudioClip은 유니티에서 음악을 재생할 파일을 의미하는 것 같다.
/// 
/// Dictionary는 기본적으로 자동으로 직렬화가 안돼서 에디터에 안 띄워진다.
/// 그래서 serializableDictionary 패키지를 추가했다.
/// </summary>
/// 

public enum ESoundType
{
    BGM,
    SFX,
    END // 마지막을 표시하기 위해
}

public class SoundManager : MonoBehaviour
{
    /// ------------------------------------- public --------------------------------
    public void Play(string soundName, ESoundType eSoundType)
    {
        if (!clips.ContainsKey(soundName) || clips[soundName] == null)
        {
            Debug.Log($"soundName: \"{soundName}\",이름을 가진 AudioClip이 존재하지 않습니다. ");
            return;
        }

        // AudioSource 가져오기
        AudioSource source;
        if (audioSourcePool.Count > 0)
        {
            source = audioSourcePool.Dequeue();
        }
        else
        {
            source = CreateNewSource();
        }
        source.gameObject.SetActive(true);

        // SoundType에 따라 Loop 설정
        if (eSoundType == ESoundType.BGM)
        {
            source.loop = true;
            source.outputAudioMixerGroup = bgmAudioMixerGroup;
        }
        else // 효과음
        {
            source.loop = false;
            source.outputAudioMixerGroup = sfxAudioMixerGroup;
        }
        source.spatialBlend = 0;
        source.clip = clips[soundName];
        source.volume = 1.0f;
        source.Play();

        playingAudioSource[soundName] = source;
        // BGM의 경우 Loop를 돌기 때문에 바로 source를 반납하지 않는다.
        if (eSoundType == ESoundType.SFX)
        {
            StartCoroutine(ReturnToPool(source, soundName, clips[soundName].length));
        }
    }

    public void Play3DSound(string soundName, Vector3 position)
    {
        if (!clips.ContainsKey(soundName) || clips[soundName] == null)
        {
            Debug.Log($"soundName: \"{soundName}\",이름을 가진 AudioClip이 존재하지 않습니다. ");
            return;
        }

        AudioSource source;
        if (audioSourcePool.Count > 0)
        {
            source = audioSourcePool.Dequeue();
        }
        else
        {
            source = CreateNewSource();
        }
        source.gameObject.SetActive(true);

        source.outputAudioMixerGroup = sfxAudioMixerGroup;
        source.transform.position = position;
        source.clip = clips[soundName];
        source.Play();
        source.spatialBlend = 1;

        playingAudioSource[soundName] = source;
        StartCoroutine(ReturnToPool(source, soundName, clips[soundName].length));
    }

    public void Stop(string soundName)
    {
        if (!playingAudioSource.ContainsKey(soundName) || playingAudioSource[soundName] == null)
        {
            Debug.Log($"soundName: \"{soundName}\", 해당 sound는 재생하고 있지 않습니다.");
            return;
        }
        playingAudioSource[soundName].Stop();

        playingAudioSource[soundName].gameObject.SetActive(false);
        audioSourcePool.Enqueue(playingAudioSource[soundName]);
        playingAudioSource[soundName] = null;
    }

    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat(masterVolumeStr, volume);

        masterMute.isOn = false;
    }

    public void SetBGMVolume(float volume)
    {
        audioMixer.SetFloat("BGM", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat(BGMVolumeStr, volume);

        BGMMute.isOn = false;
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat(SFXVolumeStr, volume);

        SFXMute.isOn = false;
    }

    public void SetMasterMute(bool isOn)
    {
        if(isOn)
        {
            audioMixer.SetFloat("Master", Mathf.Log10(0.001f) * 20);
        }
        else
        {
            SetMasterVolume(masterSlider.value);
        }
        PlayerPrefs.SetInt(masterMuteStr, isOn ? 1 : 0);
    }

    public void SetBGMMute(bool isOn)
    {
        if (isOn)
        {
            audioMixer.SetFloat("BGM", Mathf.Log10(0.001f) * 20);
        }
        else
        {
            SetBGMVolume(BGMSlider.value);
        }
        PlayerPrefs.SetInt(BGMMuteStr, isOn ? 1 : 0);
    }

    public void SetSFXMute(bool isOn)
    {
        if (isOn)
        {
            audioMixer.SetFloat("SFX", Mathf.Log10(0.001f) * 20);
        }
        else
        {
            SetSFXVolume(SFXSlider.value);
        }
        PlayerPrefs.SetInt(SFXMuteStr, isOn ? 1 : 0);
    }

    /// ------------------------------------- private --------------------------------
    // PoolSize는 플젝할 때 로그 남기면서 조절할 필요가 있을 듯
    private const int audioSourcePoolSize = 50;

    // 볼륨 조절
    [Header("Audio Mixer Setting")]
    [SerializeField] private AudioMixer audioMixer;

    [SerializeField] private AudioMixerGroup masterAudioMixerGroup; // audioMixer.find 로 가져와도 되긴 하는데 바로 접근이 안되고 검색해서 가져오는 거 같아서 그냥 메모리에 저장하기로 함
    [SerializeField] private AudioMixerGroup bgmAudioMixerGroup;
    [SerializeField] private AudioMixerGroup sfxAudioMixerGroup;

    [Header("Audio UI 연결")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider BGMSlider;
    [SerializeField] private Slider SFXSlider;
    [SerializeField] private Toggle masterMute;
    [SerializeField] private Toggle BGMMute;
    [SerializeField] private Toggle SFXMute;

    [Header("Sound 관리")]
    // Sound 관리
    [SerializeField] private SerializableDictionary<string, AudioClip> clips;

    [SerializeField] private AudioSource audioSourcePrefab;
    private Queue<AudioSource> audioSourcePool = new Queue<AudioSource>();
    private Dictionary<string, AudioSource> playingAudioSource = new Dictionary<string, AudioSource>(); // 해당 string에 해당하는 clip을 재생중인 source

    private string masterVolumeStr = "MasterVolume";
    private string BGMVolumeStr = "BGMVolume";
    private string SFXVolumeStr = "SFXVolume";
    private string masterMuteStr = "MasterMute";
    private string BGMMuteStr = "BGMMute";
    private string SFXMuteStr = "SFXMute";

    // test용 - 3D 사운드 
    //Vector3 soundPlayPosition;
    //float delay = 0.0f;

    private void Awake()
    {
        for (int i = 0; i < audioSourcePoolSize; i++)
        {
            CreateNewSource();
        }

        if(masterSlider)
        {
            LoadVolume();
            ConnectUI();
        }
    }

    private void Start()
    {
        // Test용 - 일반 사운드 효과음, 배경음
        //Play("BGM", ESoundType.BGM);
        //Play("SFX", ESoundType.SFX);
    }

    private void Update()
    {
        // Test용 - 3D 사운드 
        //delay += Time.deltaTime;
        //
        //if(delay >= 1.0f)
        //{
        //    soundPlayPosition.x += 10.0f;
        //    //if (Input.GetKeyDown(KeyCode.A))
        //    {
        //        Play3DSound("SFX", soundPlayPosition);
        //    }
        //
        //    delay = 0.0f;
        //}

        //if(Input.GetKeyDown(KeyCode.T))
        //{
        //    Stop("BGM");
        //}
        //
        //if (Input.GetKeyDown(KeyCode.D))
        //{
        //    Play("TestError", ESoundType.BGM);
        //}
        //
        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    Play("SFX", ESoundType.SFX);
        //}
    }

    private void ConnectUI()
    {
        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        BGMSlider.onValueChanged.AddListener(SetBGMVolume);
        SFXSlider.onValueChanged.AddListener(SetSFXVolume);

        masterMute.onValueChanged.AddListener(SetMasterMute);
        BGMMute.onValueChanged.AddListener(SetBGMMute);
        SFXMute.onValueChanged.AddListener(SetSFXMute);
    }

    private void LoadVolume()
    {
        // volume 불러오기
        if (PlayerPrefs.HasKey(masterVolumeStr))
        {
            masterSlider.value = PlayerPrefs.GetFloat(masterVolumeStr);
        }
        else
        {
            masterSlider.value = 0.7f;
        }
        if (PlayerPrefs.HasKey(BGMVolumeStr))
        {
            BGMSlider.value = PlayerPrefs.GetFloat(BGMVolumeStr);
        }
        else
        {
            BGMSlider.value = 0.7f;
        }
        if (PlayerPrefs.HasKey(SFXVolumeStr))
        {
            SFXSlider.value = PlayerPrefs.GetFloat(SFXVolumeStr);
        }
        else
        {
            SFXSlider.value = 0.7f;
        }
        // Mute 불러오기
        if (PlayerPrefs.HasKey(masterMuteStr))
        {
            masterMute.isOn = PlayerPrefs.GetInt(masterMuteStr) == 1;
        }
        else
        {
            masterMute.isOn = false;
        }
        if (PlayerPrefs.HasKey(BGMMuteStr))
        {
            BGMMute.isOn = PlayerPrefs.GetFloat(BGMMuteStr) == 1;
        }
        else
        {
            BGMMute.isOn = false;
        }
        if (PlayerPrefs.HasKey(SFXMuteStr))
        {
            SFXMute.isOn = PlayerPrefs.GetFloat(SFXMuteStr) == 1;
        }
        else
        {
            SFXMute.isOn = false;
        }
    }

    private AudioSource CreateNewSource()
    {
        AudioSource newSource = Instantiate(audioSourcePrefab, transform);
        newSource.gameObject.SetActive(false);
        audioSourcePool.Enqueue(newSource);

        return newSource;
    }

    // 일정 시간 후 다시 풀에 반환
    private IEnumerator ReturnToPool(AudioSource source, string soundName,  float delay)
    {
        yield return new WaitForSeconds(delay);
        source.gameObject.SetActive(false);
        audioSourcePool.Enqueue(source);
        playingAudioSource[soundName] = null;
    }
}

