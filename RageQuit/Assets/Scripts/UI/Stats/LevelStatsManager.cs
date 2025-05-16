using UnityEngine;
using Mono.Data.Sqlite;
using System.Collections.Generic;

public class LevelStatsManager : MonoBehaviour
{
    public static LevelStatsManager Instance { get; private set; }

    private int muertes;
    private int idProgreso;
    private string dbPath;

    private float tiempoAcumuladoEnNivel = 0f;

    private static HashSet<string> escenasReiniciadas = new HashSet<string>();

    public int IdNivel => NivelMap.GetIdNivelPorNombre(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    public float TiempoActual => tiempoAcumuladoEnNivel;
    public int MuertesActuales => muertes;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            dbPath = "URI=file:" + Application.persistentDataPath + "/RageQuitDB.db";
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // Acumula el tiempo manualmente con deltaTime
        tiempoAcumuladoEnNivel += Time.deltaTime;
    }

    public void Inicializar()
    {
        idProgreso = GameSession.Instance.IdProgreso;
        muertes = 0;
        tiempoAcumuladoEnNivel = 0f;  // Reinicia tiempo cuando inicias nivel
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
        tiempoAcumuladoEnNivel = 0f; // Reinicia también el tiempo aquí para que no persista si quieres
        escenasReiniciadas.Add(escenaActual);
        Debug.Log("Estadísticas reiniciadas.");
    }

    public void RegistrarMuerte()
    {
        muertes++;
        Debug.Log($"Muertes registradas: {muertes}");
        RegistrarMuerteEnDB();  // Tu método para guardar en BD
    }


    private void RegistrarMuerteEnDB()
    {
        using (var conexion = new SqliteConnection(dbPath))
        {
            conexion.Open();

            bool existeRegistro = false;

            // Verificar si existe registro para este nivel y progreso
            using (var cmdCheck = conexion.CreateCommand())
            {
                cmdCheck.CommandText = @"
                SELECT 1 FROM EstadisticasNivel 
                WHERE idNivel = @nivel AND idProgreso = @progreso
                LIMIT 1";
                cmdCheck.Parameters.AddWithValue("@nivel", IdNivel);
                cmdCheck.Parameters.AddWithValue("@progreso", idProgreso);

                var result = cmdCheck.ExecuteScalar();
                existeRegistro = (result != null);
            }

            if (existeRegistro)
            {
                // Si existe, actualizar muertes sumando 1
                using (var cmdUpdate = conexion.CreateCommand())
                {
                    cmdUpdate.CommandText = @"
                    UPDATE EstadisticasNivel 
                    SET muertes = muertes + 1 
                    WHERE idNivel = @nivel AND idProgreso = @progreso";
                    cmdUpdate.Parameters.AddWithValue("@nivel", IdNivel);
                    cmdUpdate.Parameters.AddWithValue("@progreso", idProgreso);
                    cmdUpdate.ExecuteNonQuery();
                }
            }
            else
            {
                // Si no existe, insertar nuevo registro con muertes=1 y tiempos iniciales
                using (var cmdInsert = conexion.CreateCommand())
                {
                    cmdInsert.CommandText = @"
                    INSERT INTO EstadisticasNivel (idProgreso, idNivel, muertes, tiempo, mejorTiempo) 
                    VALUES (@progreso, @nivel, 1, 0, 999999)";
                    cmdInsert.Parameters.AddWithValue("@progreso", idProgreso);
                    cmdInsert.Parameters.AddWithValue("@nivel", IdNivel);
                    cmdInsert.ExecuteNonQuery();
                }
                Debug.Log("[DEBUG] Primer muerte en este nivel, se insertó nuevo registro en EstadisticasNivel.");
            }

            // Actualizar muertes totales en ProgresoJugador
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

        Debug.Log("[DEBUG] Muerte registrada en DB correctamente.");
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
