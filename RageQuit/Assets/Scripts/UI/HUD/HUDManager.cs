using UnityEngine;
using TMPro;
using System;
public class HUDManager : MonoBehaviour
{
    public TextMeshProUGUI muerteText;      // Texto para mostrar las muertes actuales
    public TextMeshProUGUI tiempoText;      // Texto para mostrar el tiempo jugado actual
    public TextMeshProUGUI nivelActualText; // Texto para mostrar el nombre del nivel actual

    void Update()
    {
        if (LevelStatsManager.Instance == null) return; // Salir si no hay instancia disponible

        // Obtener las muertes actuales desde el LevelStatsManager
        int muertes = LevelStatsManager.Instance.MuertesActuales;

        // Obtener el tiempo actual desde el LevelStatsManager
        float tiempo = LevelStatsManager.Instance.TiempoActual;

        // Obtener el nombre del nivel activo en la escena
        string nombreNivel = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        // Actualizar el texto en el HUD con los valores actuales
        muerteText.text = "Deaths: " + muertes;
        tiempoText.text = "Time: " + tiempo.ToString("F2") + "s"; // Formato con 2 decimales
        nivelActualText.text = nombreNivel;
    }
}
