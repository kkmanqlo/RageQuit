using UnityEngine;
using Mono.Data.Sqlite;
using System.Collections.Generic;

public class LevelStatsManager : MonoBehaviour
{
    // Instancia Singleton para acceso global
    public static LevelStatsManager Instance { get; private set; }

    // Contador de muertes en el nivel actual
    private int muertes;
    // ID del progreso actual (slot/progreso del jugador)
    private int idProgreso;
    // Ruta a la base de datos SQLite
    private string dbPath;

    // Tiempo acumulado en el nivel actual, en segundos
    private float tiempoAcumuladoEnNivel = 0f;

    // HashSet para controlar qué escenas ya tuvieron reinicio de estadísticas
    private static HashSet<string> escenasReiniciadas = new HashSet<string>();

    // Obtiene el ID del nivel basado en el nombre de la escena actual
    public int IdNivel => NivelMap.GetIdNivelPorNombre(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);

    // Propiedad pública para obtener el tiempo acumulado actual
    public float TiempoActual => tiempoAcumuladoEnNivel;

    // Propiedad pública para obtener las muertes actuales
    public int MuertesActuales => muertes;

    void Awake()
    {
        if (Instance == null)
        {
            // Si no existe instancia, asignar esta y no destruir al cambiar escena
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // Inicializar la ruta de la base de datos
            dbPath = "URI=file:" + Application.persistentDataPath + "/RageQuitDB.db";
        }
        else
        {
            // Si ya existe instancia, destruir esta para evitar duplicados
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // Sumar el deltaTime a tiempo acumulado para medir duración del nivel
        tiempoAcumuladoEnNivel += Time.deltaTime;
    }

    // Inicializa las estadísticas para un nuevo intento de nivel
    public void Inicializar()
    {
        // Obtener idProgreso del GameSession actual
        idProgreso = GameSession.Instance.IdProgreso;
        // Reiniciar muertes y tiempo acumulado
        muertes = 0;
        tiempoAcumuladoEnNivel = 0f;
        Debug.Log($"Inicializando LevelStatsManager. idProgreso: {idProgreso}");
    }

    // Método estático para limpiar el estado de reinicio para una escena dada
    public static void PrepararCargaDeNivel(string nombreEscena)
    {
        // Permite reiniciar estadísticas de nuevo cuando se cargue la escena
        escenasReiniciadas.Remove(nombreEscena);
    }

    // Reinicia las estadísticas de muertes y tiempo solo una vez por escena cargada
    public void ReiniciarEstadisticas()
    {
        string escenaActual = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        // Si ya se reiniciaron antes, no hacer nada para evitar reinicios múltiples
        if (escenasReiniciadas.Contains(escenaActual))
        {
            Debug.Log("Ya se reiniciaron las estadísticas para esta escena.");
            return;
        }

        // Reiniciar muertes y tiempo acumulado a cero
        muertes = 0;
        tiempoAcumuladoEnNivel = 0f;
        // Marcar la escena como reiniciada
        escenasReiniciadas.Add(escenaActual);
        Debug.Log("Estadísticas reiniciadas.");
    }

    // Incrementa contador de muertes y guarda la actualización en base de datos
    public void RegistrarMuerte()
    {
        muertes++;
        Debug.Log($"Muertes registradas: {muertes}");
        RegistrarMuerteEnDB();  // Llamar método que actualiza BD
    }

    // Guarda o actualiza en la base de datos la muerte actual para nivel y progreso
    private void RegistrarMuerteEnDB()
    {
        using (var conexion = new SqliteConnection(dbPath))
        {
            conexion.Open();

            bool existeRegistro = false;

            // Verificar si ya existe registro para este nivel y progreso
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
                // Si existe registro, actualizar sumando 1 a muertes
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
                // Si no existe, insertar un nuevo registro con muertes = 1 y tiempos iniciales
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

            // Actualizar muertes totales acumuladas en ProgresoJugador para este progreso
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

    // Guarda el tiempo final del nivel en la base de datos y actualiza mejor tiempo si corresponde
    public void GuardarTiempoFinal()
    {
        float tiempoFinal = TiempoActual;
        Debug.Log($"[DEBUG] GuardarTiempoFinal() llamado. Tiempo: {tiempoFinal}");

        using (var conexion = new SqliteConnection(dbPath))
        {
            conexion.Open();

            bool existeRegistro = false;
            float mejorTiempoPrevio = 0f;

            // Buscar el mejor tiempo previo para comparar
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
                // Actualizar tiempo actual y mejor tiempo si es menor
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
                // Si no existe registro, insertar uno nuevo con el tiempo actual
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

            // Actualizar el tiempo total acumulado en ProgresoJugador sumando el tiempo actual
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

