using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class TriggerController : MonoBehaviour
{
    public Toggle toggle;                      // Toggle para activar/desactivar pantalla completa

    public TMP_Dropdown resolutionDropdown;   // Dropdown para seleccionar resolución
    Resolution[] resolutions;                  // Array para almacenar las resoluciones soportadas por la pantalla

    // Start se ejecuta una vez al inicio
    void Start()
    {
        // Ajustar el toggle según si la pantalla está en modo full screen o no
        if (Screen.fullScreen)
        {
            toggle.isOn = true;
        }
        else
        {
            toggle.isOn = false;
        }

        CheckResolutions();  // Cargar y mostrar las resoluciones disponibles en el dropdown
    }

    // Método llamado cuando se cambia el toggle para activar/desactivar pantalla completa
    public void ActivateFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

    // Método para cargar las resoluciones soportadas y mostrarlas en el dropdown
public void CheckResolutions()
{
    resolutions = Screen.resolutions;
    resolutionDropdown.ClearOptions();

    int currentResolutionIndex = 0;
    List<string> options = new List<string>();
    List<Resolution> uniqueResolutions = new List<Resolution>();
    HashSet<string> seenResolutions = new HashSet<string>();

    for (int i = 0; i < resolutions.Length; i++)
    {
        string resString = resolutions[i].width + "x" + resolutions[i].height;

        if (!seenResolutions.Contains(resString))
        {
            seenResolutions.Add(resString);
            uniqueResolutions.Add(resolutions[i]);
        }
    }

    // 🔽 Ordenar de mayor a menor resolución
    uniqueResolutions.Sort((a, b) =>
    {
        if (a.width != b.width)
            return b.width.CompareTo(a.width);
        else
            return b.height.CompareTo(a.height);
    });

    // Generar opciones ya ordenadas
    for (int i = 0; i < uniqueResolutions.Count; i++)
    {
        string option = uniqueResolutions[i].width + " x " + uniqueResolutions[i].height;
        options.Add(option);

        if (Screen.fullScreen &&
            uniqueResolutions[i].width == Screen.currentResolution.width &&
            uniqueResolutions[i].height == Screen.currentResolution.height)
        {
            currentResolutionIndex = i;
        }
    }

    resolutionDropdown.AddOptions(options);
    resolutionDropdown.value = PlayerPrefs.GetInt("ResolutionIndex", currentResolutionIndex);
    resolutionDropdown.RefreshShownValue();

    resolutions = uniqueResolutions.ToArray();

    SetResolution(resolutionDropdown.value);
}

    // Método llamado cuando el usuario selecciona una resolución en el dropdown
    public void SetResolution(int resolutionIndex)
    {
        PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);

        Resolution resolution = resolutions[resolutionIndex];

        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
}


