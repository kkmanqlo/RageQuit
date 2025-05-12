using UnityEngine;

public class ResetStats : MonoBehaviour
{
    void Start()
    {
        LevelStatsManager.Instance.ReiniciarEstadisticas();
    }
}
