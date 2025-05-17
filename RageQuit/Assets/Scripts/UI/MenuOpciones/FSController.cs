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
        resolutions = Screen.resolutions;  // Obtener todas las resoluciones soportadas
        resolutionDropdown.ClearOptions(); // Limpiar opciones anteriores del dropdown
        int currentResolutionIndex = 0;    // Índice para seleccionar la resolución actual
        List<string> options = new List<string>(); // Lista de strings para mostrar en el dropdown

        // Iterar todas las resoluciones para crear las opciones de texto
        for (int i = 0; i < resolutions.Length; i++)
        {
            // Crear texto como "1920 x 1080"
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            // Si la resolución es la misma que la actual, guardar el índice
            if (Screen.fullScreen && resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        // Agregar las opciones al dropdown
        resolutionDropdown.AddOptions(options);

        // Seleccionar la opción que coincide con la resolución actual
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        // Finalmente, cargar la resolución guardada en PlayerPrefs si existe, si no, usa 0
        resolutionDropdown.value = PlayerPrefs.GetInt("ResolutionIndex", 0);
    }

    // Método llamado cuando el usuario selecciona una resolución en el dropdown
    public void SetResolution(int resolutionIndex)
    {
        // Guardar la resolución seleccionada para que se recuerde en futuras ejecuciones
        PlayerPrefs.SetInt("ResolutionIndex", resolutionDropdown.value);

        // Obtener la resolución seleccionada
        Resolution resolution = resolutions[resolutionIndex];

        // Aplicar la resolución y mantener el estado de pantalla completa
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
}


