using UnityEngine;
using Mono.Data.Sqlite;

public class LevelSelectorManager : MonoBehaviour
{
    private string dbPath;

    void Start()
    {
        // Define la ruta a la base de datos SQLite
        dbPath = "URI=file:" + Application.persistentDataPath + "/RageQuitDB.db";

        // Carga el progreso guardado para el usuario y slot actuales
        CargarIdProgreso();

        // Inicializa las estadísticas de nivel (por ejemplo, muertes y tiempo)
        LevelStatsManager.Instance.Inicializar(); 
    }

    void CargarIdProgreso()
    {
        // Obtiene el ID del usuario actual y el slot seleccionado
        int idUsuario = UsuarioManager.Instance.IdUsuario;
        int slot = GameSession.Instance.SlotSeleccionado;

        using (var conexion = new SqliteConnection(dbPath))
        {
            conexion.Open();
            using (var cmd = conexion.CreateCommand())
            {
                // Prepara la consulta para obtener el idProgreso según usuario y slot
                cmd.CommandText = @"
                    SELECT idProgreso FROM ProgresoJugador 
                    WHERE idUsuario = @id AND slotNumero = @slot";
                cmd.Parameters.AddWithValue("@id", idUsuario);
                cmd.Parameters.AddWithValue("@slot", slot);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Si encuentra el progreso, lo guarda en GameSession
                        int idProgreso = reader.GetInt32(0);
                        GameSession.Instance.IdProgreso = idProgreso;

                        Debug.Log("idProgreso cargado: " + idProgreso);
                    }
                    else
                    {
                        // Si no se encuentra, muestra error en consola
                        Debug.LogError("No se encontró el progreso para el slot: " + slot);
                    }
                }
            }
        }
    }
}
