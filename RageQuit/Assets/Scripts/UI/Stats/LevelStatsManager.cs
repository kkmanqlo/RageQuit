using UnityEngine;
using Mono.Data.Sqlite;

public class LevelStatsManager : MonoBehaviour
{
    public static LevelStatsManager Instance;

    private float tiempoNivel;
    private int muertes;
    private int idNivel;
    private int idProgreso;

    private string dbPath;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 🔥 Mantiene el objeto entre escenas
        }
        else
        {
            Destroy(gameObject);
        }
    }


    void Start()
    {
        dbPath = "URI=file:" + Application.persistentDataPath + "/RageQuitDB.db";
        string escenaActual = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        if (escenaActual.StartsWith("Nivel"))
        {
            // Inicializa las estadísticas del nivel
            tiempoNivel = 0f;
            muertes = 0;
            idProgreso = GameSession.Instance.IdProgreso;
            idNivel = NivelMap.GetIdNivelPorNombre(escenaActual);
        }
        else if (escenaActual == "MenuPrincipal" || escenaActual == "DataSaveSelectionScene" || escenaActual == "LevelSelectionScene")
        {
            tiempoNivel = 0f;
            muertes = 0;
        }
        
    }

    void Update()
    {
        tiempoNivel += Time.deltaTime;
    }

    public void RegistrarMuerte()
    {
        muertes++;
    }

    public void GuardarEstadisticas()
    {
        using (var conexion = new SqliteConnection(dbPath))
        {
            conexion.Open();

            bool existeRegistro = false;
            int muertesPrevias = 0;
            float mejorTiempoPrevio = 0f;

            // Leer si ya hay estadísticas
            using (var cmdCheck = conexion.CreateCommand())
            {
                cmdCheck.CommandText = "SELECT muertes, mejorTiempo FROM EstadisticasNivel WHERE idNivel = @nivel AND idProgreso = @progreso";
                cmdCheck.Parameters.AddWithValue("@nivel", idNivel);
                cmdCheck.Parameters.AddWithValue("@progreso", idProgreso);

                using (var reader = cmdCheck.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        existeRegistro = true;
                        muertesPrevias = reader.GetInt32(0);
                        mejorTiempoPrevio = reader.GetFloat(1);
                    }
                }
            }

            if (existeRegistro)
            {
                using (var cmdUpdate = conexion.CreateCommand())
                {
                    cmdUpdate.CommandText = "UPDATE EstadisticasNivel SET muertes = @muertes, tiempo = @tiempo, mejorTiempo = @mejorTiempo WHERE idNivel = @nivel AND idProgreso = @progreso";
                    cmdUpdate.Parameters.AddWithValue("@muertes", Mathf.Min(muertes, muertesPrevias));
                    cmdUpdate.Parameters.AddWithValue("@tiempo", tiempoNivel);
                    cmdUpdate.Parameters.AddWithValue("@mejorTiempo", Mathf.Min(tiempoNivel, mejorTiempoPrevio));
                    cmdUpdate.Parameters.AddWithValue("@nivel", idNivel);
                    cmdUpdate.Parameters.AddWithValue("@progreso", idProgreso);
                    cmdUpdate.ExecuteNonQuery();
                }
            }
            else
            {
                using (var cmdInsert = conexion.CreateCommand())
                {
                    cmdInsert.CommandText = "INSERT INTO EstadisticasNivel (idProgreso, idNivel, muertes, tiempo, mejorTiempo) VALUES (@progreso, @nivel, @muertes, @tiempo, @mejorTiempo)";
                    cmdInsert.Parameters.AddWithValue("@progreso", idProgreso);
                    cmdInsert.Parameters.AddWithValue("@nivel", idNivel);
                    cmdInsert.Parameters.AddWithValue("@muertes", muertes);
                    cmdInsert.Parameters.AddWithValue("@tiempo", tiempoNivel);
                    cmdInsert.Parameters.AddWithValue("@mejorTiempo", tiempoNivel);
                    cmdInsert.ExecuteNonQuery();
                }
            }

            // Actualizar progreso general
            using (var cmdUpdateProgreso = conexion.CreateCommand())
            {
                cmdUpdateProgreso.CommandText = @"
                    UPDATE ProgresoJugador 
                    SET 
                        muertesTotales = muertesTotales + @muertes,
                        tiempoTotal = tiempoTotal + @tiempo
                    WHERE idProgreso = @idProgreso";

                cmdUpdateProgreso.Parameters.AddWithValue("@muertes", muertes);
                cmdUpdateProgreso.Parameters.AddWithValue("@tiempo", tiempoNivel);
                cmdUpdateProgreso.Parameters.AddWithValue("@idProgreso", idProgreso);
                cmdUpdateProgreso.ExecuteNonQuery();
            }
        }

        Debug.Log("📊 Estadísticas guardadas correctamente.");
    }
}
