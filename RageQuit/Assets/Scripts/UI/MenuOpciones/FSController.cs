using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class TriggerController : MonoBehaviour
{
    public Toggle toggle;

    public TMP_Dropdown resolutionDropdown;
    Resolution[] resolutions;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Screen.fullScreen)
        {

            toggle.isOn = true;
        }
        else
        {
            toggle.isOn = false;
        }

        CheckResolutions();
    }

    public void ActivateFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

    public void CheckResolutions()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        int currentResolutionIndex = 0;
        List<string> options = new List<string>();

        for (int i = 0; i < resolutions.Length; i++)
        {

            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (Screen.fullScreen && resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        resolutionDropdown.value = PlayerPrefs.GetInt("ResolutionIndex", 0);
    }

    public void SetResolution(int resolutionIndex)
    {
        PlayerPrefs.SetInt("ResolutionIndex", resolutionDropdown.value);
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
}

