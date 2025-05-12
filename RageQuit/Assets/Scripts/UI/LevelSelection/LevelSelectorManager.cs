using UnityEngine;
using Mono.Data.Sqlite;

public class LevelSelectorManager : MonoBehaviour
{
    private string dbPath;

    void Start()
    {
        dbPath = "URI=file:" + Application.persistentDataPath + "/RageQuitDB.db";
        CargarIdProgreso();
        LevelStatsManager.Instance.Inicializar();
    }

    void CargarIdProgreso()
    {
        int idUsuario = UsuarioManager.Instance.IdUsuario;
        int slot = GameSession.Instance.SlotSeleccionado;

        using (var conexion = new SqliteConnection(dbPath))
        {
            conexion.Open();
            using (var cmd = conexion.CreateCommand())
            {
                cmd.CommandText = @"
                    SELECT idProgreso FROM ProgresoJugador 
                    WHERE idUsuario = @id AND slotNumero = @slot";
                cmd.Parameters.AddWithValue("@id", idUsuario);
                cmd.Parameters.AddWithValue("@slot", slot);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int idProgreso = reader.GetInt32(0);
                        GameSession.Instance.IdProgreso = idProgreso;

                        Debug.Log("idProgreso cargado: " + idProgreso);
                    }
                    else
                    {
                        Debug.LogError("No se encontró el progreso para el slot: " + slot);
                    }
                }
            }
        }
    }
}