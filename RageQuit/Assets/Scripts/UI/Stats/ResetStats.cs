using UnityEngine;

public class ResetStats : MonoBehaviour
{
    // Este script se encarga de reiniciar las estadísticas del juego al inicio de la escena.
    // Se utiliza para restablecer los datos de estadísticas al cargar la escena correspondiente.
    void Start()
    {
        LevelStatsManager.Instance.ReiniciarEstadisticas();
    }
}
