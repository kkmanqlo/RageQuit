using UnityEngine;
using TMPro;
using System;
public class HUDManager : MonoBehaviour
{
    public TextMeshProUGUI muerteText;
    public TextMeshProUGUI tiempoText;
    public TextMeshProUGUI nivelActualText;
    


    void Update()
    {
        
        if (LevelStatsManager.Instance == null) return;

        // Obtener los valores actuales
        int muertes = LevelStatsManager.Instance.MuertesActuales;
        float tiempo = LevelStatsManager.Instance.TiempoActual;
        string nombreNivel = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        // Actualizar textos
        muerteText.text = "Muertes: " + muertes;
        tiempoText.text = "Tiempo: " + tiempo.ToString("F2") + "s";
        nivelActualText.text = nombreNivel;
    }
}
