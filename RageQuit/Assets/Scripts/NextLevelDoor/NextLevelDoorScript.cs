using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mono.Data.Sqlite;
using System.Collections;

public class NextLevelDoorScript : MonoBehaviour
{
    [Tooltip("¿Usar siguiente escena en el build index automáticamente?")]
    public bool autoLoadNextScene = true; // Determina si se carga automáticamente la siguiente escena del build index.

    [Tooltip("Si no es automático, nombra aquí la escena a cargar")]
    public string sceneToLoad; // Si no se usa el modo automático, esta es la escena a cargar manualmente.

    private bool activated = false; // Flag para evitar que se active varias veces seguidas.

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si ya fue activado, no hacer nada.
        if (activated) return;

        // Verifica si el objeto que entró es el jugador.
        CharacterMovement character = collision.GetComponent<CharacterMovement>();
        if (character != null)
        {
            activated = true;
            StartCoroutine(ProcesarVictoria()); // Inicia el proceso de victoria.
        }
    }

    // Rutina para guardar progreso y luego cargar la siguiente escena.
    private IEnumerator ProcesarVictoria()
    {
        // Actualiza el progreso del jugador en la base de datos (nivelActual).
        ActualizarProgresoEnBD();

        // Guarda las estadísticas del nivel (tiempo final y muertes).
        if (LevelStatsManager.Instance != null)
        {
            LevelStatsManager.Instance.GuardarTiempoFinal();
        }

        // Espera corta para asegurar que la DB haya terminado de guardar.
        yield return new WaitForSeconds(0.2f);

        // Carga la siguiente escena.
        LoadScene();
    }

    // Carga la escena siguiente o una específica.
    private void LoadScene()
    {
        DOTween.KillAll(); // Detiene cualquier tween activo (transiciones, animaciones, etc.).

        if (autoLoadNextScene)
        {
            // Obtiene el índice de la siguiente escena en el build settings.
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

            // Verifica si hay otra escena válida en el build index.
            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                // Obtiene el nombre de la siguiente escena (sin extensión).
                string nextSceneName = SceneUtility.GetScenePathByBuildIndex(nextSceneIndex);
                nextSceneName = System.IO.Path.GetFileNameWithoutExtension(nextSceneName);

                // Prepara los datos para el nuevo nivel.
                LevelStatsManager.PrepararCargaDeNivel(nextSceneName);

                // Carga la siguiente escena.
                SceneManager.LoadScene(nextSceneIndex);
            }
            else
            {
                Debug.LogWarning("No hay más escenas en el build index.");
            }
        }
        else
        {
            // Modo manual: Cargar escena por nombre si fue asignada.
            if (!string.IsNullOrEmpty(sceneToLoad))
            {
                LevelStatsManager.PrepararCargaDeNivel(sceneToLoad);
                SceneManager.LoadScene(sceneToLoad);
            } 
            else
            {
                Debug.LogError("No se ha asignado el nombre de la escena a cargar.");
            }
        }
    }

    // Actualiza el nivel alcanzado en la base de datos, solo si es mayor que el actual.
    private void ActualizarProgresoEnBD()
    {
        // Ruta a la base de datos.
        string dbPath = "URI=file:" + Application.persistentDataPath + "/RageQuitDB.db";

        // ID de progreso actual (slot del jugador activo).
        int idProgreso = GameSession.Instance.IdProgreso;

        // Obtenemos el ID del nivel actual desde el mapa.
        string nombreEscena = SceneManager.GetActiveScene().name;
        int idNivelActual = NivelMap.GetIdNivelPorNombre(nombreEscena);
        int siguienteNivel = idNivelActual + 1;

        // Conexión a la base de datos.
        using (var conexion = new SqliteConnection(dbPath))
        {
            conexion.Open();
            using (var cmd = conexion.CreateCommand())
            {
                // Solo actualiza si el nuevo nivel es mayor que el actual registrado.
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

