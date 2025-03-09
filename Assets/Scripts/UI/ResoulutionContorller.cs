using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResoulutionContorller : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;

    public Toggle fullToggle;
    public Toggle windowToggle;

    public Slider sizeSlider;

    private Resolution[] resolutions;

    enum ScreenMode
    {
        Full,
        Window
    }
    ScreenMode screenMode;


    private void Start()
    {
        SetUpDropdown();
        SetUpToggles();
    }

    void SetUpDropdown()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        HashSet<string> options = new HashSet<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " X " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(new List<string>(options));
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution()
    {
        int resolutionIndex = resolutionDropdown.value;
        Resolution resolution = resolutions[resolutionIndex];
        
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        Debug.Log(Screen.width + " X " + Screen.height);
    }

    void SetUpToggles()
    {
        fullToggle.isOn = true;
        windowToggle.isOn = false;

        fullToggle.onValueChanged.AddListener(delegate { ToggleChanged(fullToggle); } );
        windowToggle.onValueChanged.AddListener(delegate { ToggleChanged(windowToggle); });
    }

    void ToggleChanged(Toggle changedToggle)
    {
        if(changedToggle.isOn)
        {
            if (changedToggle == fullToggle)
            {
                windowToggle.isOn = false;
                screenMode = ScreenMode.Full;
            }
            else
            {
                fullToggle.isOn = false;
                screenMode = ScreenMode.Window;
            }
        }
    }

    public void SetScreenMode()
    {
        if(screenMode == ScreenMode.Full)
        {
            Screen.SetResolution(Screen.width, Screen.height, true);
        }
        else
        {
            Screen.SetResolution(Screen.width, Screen.height, false);
        }
    }

}
