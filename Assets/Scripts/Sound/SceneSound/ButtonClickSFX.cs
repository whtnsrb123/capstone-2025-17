using UnityEngine;
using UnityEngine.UI;

public class ButtonClickSFX : MonoBehaviour
{
    [SerializeField] private SoundManager soundManager;

    Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClickButton);

        FindSoundManager();

        Debug.Assert(button != null, "this button is null!");
        Debug.Assert(soundManager != null, "soundManager is null!");
    }

    private void FindSoundManager()
    {
        if (soundManager == null)
        {
            soundManager = FindAnyObjectByType<SoundManager>();
            Debug.Assert(soundManager != null, "sound manager is null!");
        }
    }

    public void OnClickButton()
    {
        if (soundManager == null)
        {
            FindSoundManager() ;
        }
        soundManager.Play("buttonClick", ESoundType.SFX);
    }

}