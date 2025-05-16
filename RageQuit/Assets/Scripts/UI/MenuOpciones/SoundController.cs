using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundController : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    void Start()
    {
        // Cargar volumenes guardados o establecer valores iniciales
        float masterVol = PlayerPrefs.GetFloat("MasterVolume", 0.75f);
        float musicVol = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 0.75f);

        masterSlider.value = masterVol;
        musicSlider.value = musicVol;
        sfxSlider.value = sfxVol;

        SetMasterVolume(masterVol);
        SetMusicVolume(musicVol);
        SetSFXVolume(sfxVol);
    }

    public void SetMasterVolume(float volume)
    {
        volume = Mathf.Clamp(volume, 0.0001f, 1f); // evita 0
        audioMixer.SetFloat("MasterVolume", 20 * Mathf.Log10(volume));
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        volume = Mathf.Clamp(volume, 0.0001f, 1f); // evita 0
        audioMixer.SetFloat("MusicVolume", 20 * Mathf.Log10(volume));
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        volume = Mathf.Clamp(volume, 0.0001f, 1f); // evita 0
        audioMixer.SetFloat("SFXVolume", 20 * Mathf.Log10(volume));
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

}
