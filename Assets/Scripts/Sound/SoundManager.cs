using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UI;

/// <summary>
/// 2025.03.03 KimYuJin
/// Slider�� min Value�� 0.001f�� ����ؾ� �Ѵ�. 
/// Sound(3D ����) ��� �� ������ ����ϴ� Ŭ���� 
/// 
/// 3D Sound�� �ʿ��� Sound - ĳ���Ͱ� ���� ȿ����, NPC�� ��ü�� �Ҹ� ȿ���� ���� 
/// �ٸ��� ����ؾ���. -> AudioSourceInstance.PlayClipAtPoint(clip, Vector3)�� ����ϸ� ����� �� ����
/// �ٵ� �� �Լ��� ���� ������ ��ü�� �����ϰ� ��� ������ �Ҹ��ϴ� �� ���Ƽ� 
/// ���� audiosource pool�� ���� �����ϴ� �� ���� ���� ���� �� ���ٴ� ������ ���
/// 
/// SoundManager�� ����ϴ� ���� �� �ϳ��� ȿ���� ���� ��� ������Ʈ���� 
/// ����ϸ� ������Ʈ�� ����� �� �Ҹ��� ����� ����
/// �׸��� Sound Source�� Clip�� �� �������� �����ؼ� ���ϴٴ� ������ �ִ�.
/// 
/// Audio Source�� Sound�� ������ִ� Unity�� ������Ʈ�̴�.
/// AudioClip�� ����Ƽ���� ������ ����� ������ �ǹ��ϴ� �� ����.
/// 
/// Dictionary�� �⺻������ �ڵ����� ����ȭ�� �ȵż� �����Ϳ� �� �������.
/// �׷��� serializableDictionary ��Ű���� �߰��ߴ�.
/// </summary>
/// 

public enum ESoundType
{
    BGM,
    SFX,
    END // �������� ǥ���ϱ� ����
}

public class SoundManager : MonoBehaviour
{
    /// ------------------------------------- public --------------------------------
    public void Play(string soundName, ESoundType eSoundType)
    {
        if (!clips.ContainsKey(soundName) || clips[soundName] == null)
        {
            Debug.Log($"soundName: \"{soundName}\",�̸��� ���� AudioClip�� �������� �ʽ��ϴ�. ");
            return;
        }

        // AudioSource ��������
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

        // SoundType�� ���� Loop ����
        if (eSoundType == ESoundType.BGM)
        {
            source.loop = true;
            source.outputAudioMixerGroup = bgmAudioMixerGroup;
        }
        else // ȿ����
        {
            source.loop = false;
            source.outputAudioMixerGroup = sfxAudioMixerGroup;
        }
        source.spatialBlend = 0;
        source.clip = clips[soundName];
        source.volume = 1.0f;
        source.Play();

        playingAudioSource[soundName] = source;
        // BGM�� ��� Loop�� ���� ������ �ٷ� source�� �ݳ����� �ʴ´�.
        if (eSoundType == ESoundType.SFX)
        {
            StartCoroutine(ReturnToPool(source, soundName, clips[soundName].length));
        }
    }

    public void Play3DSound(string soundName, Vector3 position)
    {
        if (!clips.ContainsKey(soundName) || clips[soundName] == null)
        {
            Debug.Log($"soundName: \"{soundName}\",�̸��� ���� AudioClip�� �������� �ʽ��ϴ�. ");
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
            Debug.Log($"soundName: \"{soundName}\", �ش� sound�� ����ϰ� ���� �ʽ��ϴ�.");
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
    // PoolSize�� ������ �� �α� ����鼭 ������ �ʿ䰡 ���� ��
    private const int audioSourcePoolSize = 50;

    // ���� ����
    [Header("Audio Mixer Setting")]
    [SerializeField] private AudioMixer audioMixer;

    [SerializeField] private AudioMixerGroup masterAudioMixerGroup; // audioMixer.find �� �����͵� �Ǳ� �ϴµ� �ٷ� ������ �ȵǰ� �˻��ؼ� �������� �� ���Ƽ� �׳� �޸𸮿� �����ϱ�� ��
    [SerializeField] private AudioMixerGroup bgmAudioMixerGroup;
    [SerializeField] private AudioMixerGroup sfxAudioMixerGroup;

    [Header("Audio UI ����")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider BGMSlider;
    [SerializeField] private Slider SFXSlider;
    [SerializeField] private Toggle masterMute;
    [SerializeField] private Toggle BGMMute;
    [SerializeField] private Toggle SFXMute;

    [Header("Sound ����")]
    // Sound ����
    [SerializeField] private SerializableDictionary<string, AudioClip> clips;

    [SerializeField] private AudioSource audioSourcePrefab;
    private Queue<AudioSource> audioSourcePool = new Queue<AudioSource>();
    private Dictionary<string, AudioSource> playingAudioSource = new Dictionary<string, AudioSource>(); // �ش� string�� �ش��ϴ� clip�� ������� source

    private string masterVolumeStr = "MasterVolume";
    private string BGMVolumeStr = "BGMVolume";
    private string SFXVolumeStr = "SFXVolume";
    private string masterMuteStr = "MasterMute";
    private string BGMMuteStr = "BGMMute";
    private string SFXMuteStr = "SFXMute";

    // test�� - 3D ���� 
    //Vector3 soundPlayPosition;
    //float delay = 0.0f;

    private void Awake()
    {
        for (int i = 0; i < audioSourcePoolSize; i++)
        {
            CreateNewSource();
        }

        LoadVolume();
        ConnectUI();
    }

    private void Start()
    {
        // Test�� - �Ϲ� ���� ȿ����, �����
        Play("BGM", ESoundType.BGM);
        Play("SFX", ESoundType.SFX);
    }

    private void Update()
    {
        // Test�� - 3D ���� 
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

        if(Input.GetKeyDown(KeyCode.T))
        {
            Stop("BGM");
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            Play("TestError", ESoundType.BGM);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            Play("SFX", ESoundType.SFX);
        }
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
        // volume �ҷ�����
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
        // Mute �ҷ�����
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

    // ���� �ð� �� �ٽ� Ǯ�� ��ȯ
    private IEnumerator ReturnToPool(AudioSource source, string soundName,  float delay)
    {
        yield return new WaitForSeconds(delay);
        source.gameObject.SetActive(false);
        audioSourcePool.Enqueue(source);
        playingAudioSource[soundName] = null;
    }
}

