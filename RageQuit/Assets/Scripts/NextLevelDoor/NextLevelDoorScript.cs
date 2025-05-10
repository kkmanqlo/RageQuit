using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mono.Data.Sqlite;// Asegúrate de que el namespace sea correcto

public class NextLevelDoorScript : MonoBehaviour
{
    [Tooltip("¿Usar siguiente escena en el build index automáticamente?")]
    public bool autoLoadNextScene = true;

    [Tooltip("Si no es automático, nombra aquí la escena a cargar")]
    public string sceneToLoad;

    private bool activated = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (activated) return;

        CharacterMovement character = collision.GetComponent<CharacterMovement>();
        if (character != null)
        {
            activated = true;
            Invoke(nameof(LoadScene), 0.5f);
        }

        ActualizarProgresoEnBD();
        LevelStatsManager.Instance.GuardarEstadisticas();

    }

    private void LoadScene()
    {
        DOTween.KillAll();

        if (autoLoadNextScene)
        {
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(nextSceneIndex);
            }
            else
            {
                Debug.LogWarning("No hay más escenas en el build index.");
            }
        }
        else
        {
            if (!string.IsNullOrEmpty(sceneToLoad))
            {
                SceneManager.LoadScene(sceneToLoad);
            }
            else
            {
                Debug.LogError("No se ha asignado el nombre de la escena a cargar.");
            }
        }
    }

    private void ActualizarProgresoEnBD()
{
    string dbPath = "URI=file:" + Application.persistentDataPath + "/RageQuitDB.db";
    int idProgreso = GameSession.Instance.IdProgreso;

    string nombreEscena = SceneManager.GetActiveScene().name;
    int idNivelActual = NivelMap.GetIdNivelPorNombre(nombreEscena);
    int siguienteNivel = idNivelActual + 1;

    using (var conexion = new SqliteConnection(dbPath))
    {
        conexion.Open();
        using (var cmd = conexion.CreateCommand())
        {
            cmd.CommandText = @"
                UPDATE ProgresoJugador
                SET nivelActual = @siguienteNivel
                WHERE idProgreso = @idProgreso AND nivelActual < @siguienteNivel";

            cmd.Parameters.AddWithValue("@siguienteNivel", siguienteNivel);
            cmd.Parameters.AddWithValue("@idProgreso", idProgreso);
            cmd.ExecuteNonQuery();
        }
    }

    Debug.Log($"Progreso actualizado. Nuevo nivel actual: {siguienteNivel}");
}


}
