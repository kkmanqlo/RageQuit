using UnityEngine;
using Mono.Data.Sqlite;
using System.Collections.Generic;

public class LevelStatsManager : MonoBehaviour
{
    public static LevelStatsManager Instance;

    private int muertes;
    private int idProgreso;
    private string dbPath;

    public int IdNivel => NivelMap.GetIdNivelPorNombre(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    public float TiempoActual => Time.timeSinceLevelLoad;
    public int MuertesActuales => muertes;

    private static HashSet<string> escenasReiniciadas = new HashSet<string>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        dbPath = "URI=file:" + Application.persistentDataPath + "/RageQuitDB.db";
    }

    public void Inicializar()
    {
        idProgreso = GameSession.Instance.IdProgreso;
        muertes = 0;
        Debug.Log($"Inicializando LevelStatsManager. idProgreso: {idProgreso}");
    }

    public static void PrepararCargaDeNivel(string nombreEscena)
    {
        escenasReiniciadas.Remove(nombreEscena);
    }

    public void ReiniciarEstadisticas()
    {
        string escenaActual = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        if (escenasReiniciadas.Contains(escenaActual))
        {
            Debug.Log("Ya se reiniciaron las estadísticas para esta escena.");
            return;
        }

        muertes = 0;
        escenasReiniciadas.Add(escenaActual);
        Debug.Log("Estadísticas reiniciadas.");
    }

    public void RegistrarMuerte()
    {
        muertes++;
        Debug.Log($"Muertes registradas: {muertes}");
        RegistrarMuerteEnDB();
    }

    private void RegistrarMuerteEnDB()
    {
        using (var conexion = new SqliteConnection(dbPath))
        {
            conexion.Open();

            // Insertar o actualizar muertes en EstadisticasNivel
            using (var cmd = conexion.CreateCommand())
            {
                cmd.CommandText = @"
                INSERT INTO EstadisticasNivel (idProgreso, idNivel, muertes, tiempo, mejorTiempo)
                VALUES (@progreso, @nivel, 1, 0, 0)
                ON CONFLICT(idProgreso, idNivel)
                DO UPDATE SET muertes = muertes + 1";

                cmd.Parameters.AddWithValue("@progreso", idProgreso);
                cmd.Parameters.AddWithValue("@nivel", IdNivel);
                cmd.ExecuteNonQuery();
            }

            // Actualizar muertes totales
            using (var cmdProgreso = conexion.CreateCommand())
            {
                cmdProgreso.CommandText = @"
                UPDATE ProgresoJugador
                SET muertesTotales = muertesTotales + 1
                WHERE idProgreso = @idProgreso";

                cmdProgreso.Parameters.AddWithValue("@idProgreso", idProgreso);
                cmdProgreso.ExecuteNonQuery();
            }
        }

        Debug.Log("[DEBUG] Muerte registrada en DB (individual)");
    }

    public void GuardarTiempoFinal()
    {
        float tiempoFinal = TiempoActual;
        Debug.Log($"[DEBUG] GuardarTiempoFinal() llamado. Tiempo: {tiempoFinal}");

        using (var conexion = new SqliteConnection(dbPath))
        {
            conexion.Open();

            bool existeRegistro = false;
            float mejorTiempoPrevio = 0f;

            using (var cmdCheck = conexion.CreateCommand())
            {
                cmdCheck.CommandText = "SELECT mejorTiempo FROM EstadisticasNivel WHERE idNivel = @nivel AND idProgreso = @progreso";
                cmdCheck.Parameters.AddWithValue("@nivel", IdNivel);
                cmdCheck.Parameters.AddWithValue("@progreso", idProgreso);

                using (var reader = cmdCheck.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        existeRegistro = true;
                        mejorTiempoPrevio = reader.GetFloat(0);
                    }
                }
            }

            if (existeRegistro)
            {
                using (var cmdUpdate = conexion.CreateCommand())
                {
                    cmdUpdate.CommandText = "UPDATE EstadisticasNivel SET tiempo = @tiempo, mejorTiempo = @mejorTiempo WHERE idNivel = @nivel AND idProgreso = @progreso";
                    cmdUpdate.Parameters.AddWithValue("@tiempo", tiempoFinal);
                    cmdUpdate.Parameters.AddWithValue("@mejorTiempo", Mathf.Min(tiempoFinal, mejorTiempoPrevio));
                    cmdUpdate.Parameters.AddWithValue("@nivel", IdNivel);
                    cmdUpdate.Parameters.AddWithValue("@progreso", idProgreso);
                    cmdUpdate.ExecuteNonQuery();
                }
            }
            else
            {
                using (var cmdInsert = conexion.CreateCommand())
                {
                    cmdInsert.CommandText = "INSERT INTO EstadisticasNivel (idProgreso, idNivel, muertes, tiempo, mejorTiempo) VALUES (@progreso, @nivel, 0, @tiempo, @mejorTiempo)";
                    cmdInsert.Parameters.AddWithValue("@progreso", idProgreso);
                    cmdInsert.Parameters.AddWithValue("@nivel", IdNivel);
                    cmdInsert.Parameters.AddWithValue("@tiempo", tiempoFinal);
                    cmdInsert.Parameters.AddWithValue("@mejorTiempo", tiempoFinal);
                    cmdInsert.ExecuteNonQuery();
                }
            }

            // Actualizar tiempo total en ProgresoJugador
            using (var cmdUpdateProgreso = conexion.CreateCommand())
            {
                cmdUpdateProgreso.CommandText = @"
                UPDATE ProgresoJugador 
                SET tiempoTotal = tiempoTotal + @tiempo
                WHERE idProgreso = @idProgreso";

                cmdUpdateProgreso.Parameters.AddWithValue("@tiempo", tiempoFinal);
                cmdUpdateProgreso.Parameters.AddWithValue("@idProgreso", idProgreso);
                cmdUpdateProgreso.ExecuteNonQuery();
            }
        }

        Debug.Log($"Tiempo final guardado para idNivel: {IdNivel}, idProgreso: {idProgreso}");
    }
}
