using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundController : MonoBehaviour
{
    public AudioMixer audioMixer;      // Referencia al AudioMixer de Unity para controlar volúmenes
    public Slider masterSlider;        // Slider UI para volumen maestro
    public Slider musicSlider;         // Slider UI para volumen de música
    public Slider sfxSlider;           // Slider UI para volumen de efectos de sonido (SFX)

    void Start()
    {
        // Cargar los valores guardados de volumen o establecer valores por defecto (0.75)
        float masterVol = PlayerPrefs.GetFloat("MasterVolume", 0.75f);
        float musicVol = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 0.75f);

        // Asignar los valores cargados a los sliders para reflejar el estado actual
        masterSlider.value = masterVol;
        musicSlider.value = musicVol;
        sfxSlider.value = sfxVol;

        // Aplicar esos volúmenes al AudioMixer
        SetMasterVolume(masterVol);
        SetMusicVolume(musicVol);
        SetSFXVolume(sfxVol);
    }

    // Método para ajustar el volumen maestro
    public void SetMasterVolume(float volume)
    {
        // Clamp para evitar volumen 0 (que da log10 -∞), mínimo 0.0001
        volume = Mathf.Clamp(volume, 0.0001f, 1f);
        // Convertir volumen lineal (0 a 1) a decibelios para AudioMixer (escala logarítmica)
        audioMixer.SetFloat("MasterVolume", 20 * Mathf.Log10(volume));
        // Guardar el valor para próximas sesiones
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    // Método para ajustar volumen de música
    public void SetMusicVolume(float volume)
    {
        volume = Mathf.Clamp(volume, 0.0001f, 1f);
        audioMixer.SetFloat("MusicVolume", 20 * Mathf.Log10(volume));
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    // Método para ajustar volumen de efectos de sonido
    public void SetSFXVolume(float volume)
    {
        volume = Mathf.Clamp(volume, 0.0001f, 1f);
        audioMixer.SetFloat("SFXVolume", 20 * Mathf.Log10(volume));
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }
}

