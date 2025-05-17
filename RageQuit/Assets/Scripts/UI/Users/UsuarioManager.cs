using UnityEngine;
using Mono.Data.Sqlite;
using System;

public class UsuarioManager : MonoBehaviour
{
    // Instancia Singleton para acceso global
    public static UsuarioManager Instance;

    // Propiedad pública solo lectura para el nombre del usuario actual
    public string NombreUsuario { get; private set; }
    // Propiedad pública solo lectura para el ID del usuario actual
    public int IdUsuario { get; private set; }

    // Ruta de acceso a la base de datos SQLite
    private string dbPath;

    void Awake()
    {
        // Implementación de patrón Singleton
        if (Instance == null)
        {
            Instance = this;
            // No destruir al cargar nuevas escenas
            DontDestroyOnLoad(gameObject);
            // Definir ruta a la base de datos local
            dbPath = "URI=file:" + Application.persistentDataPath + "/RageQuitDB.db";
        }
        else
        {
            // Si ya existe una instancia, destruir esta para evitar duplicados
            Destroy(gameObject);
        }
    }

    // Método que devuelve true si hay un usuario guardado en PlayerPrefs
    public bool HayUsuarioRegistrado()
    {
        return PlayerPrefs.HasKey("IdUsuario");
    }

    // Carga el usuario guardado en PlayerPrefs y obtiene su nombre desde la base de datos
    public void CargarUsuario()
    {
        if (HayUsuarioRegistrado())
        {
            // Obtener el ID del usuario guardado
            IdUsuario = PlayerPrefs.GetInt("IdUsuario");

            // Abrir conexión a la base de datos para buscar el nombre del usuario
            using (var conexion = new SqliteConnection(dbPath))
            {
                conexion.Open();
                using (var cmd = conexion.CreateCommand())
                {
                    // Consulta SQL para obtener el nombre del usuario por su ID
                    cmd.CommandText = "SELECT nombre FROM Usuarios WHERE idUsuario = @id";
                    cmd.Parameters.AddWithValue("@id", IdUsuario);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Asignar el nombre leído de la base de datos a la propiedad pública
                            NombreUsuario = reader.GetString(0);
                        }
                    }
                }
            }
        }
    }

    // Registra un nuevo usuario en la base de datos y guarda el ID en PlayerPrefs
    public void RegistrarNuevoUsuario(string nombre)
    {
        using (var conexion = new SqliteConnection(dbPath))
        {
            conexion.Open();
            using (var cmd = conexion.CreateCommand())
            {
                // Insertar nuevo usuario con nombre y fecha actual
                cmd.CommandText = "INSERT INTO Usuarios (nombre, fechaRegistro) VALUES (@nombre, @fecha)";
                cmd.Parameters.AddWithValue("@nombre", nombre);
                cmd.Parameters.AddWithValue("@fecha", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                cmd.ExecuteNonQuery();

                // Obtener el ID autogenerado del nuevo usuario insertado
                cmd.CommandText = "SELECT last_insert_rowid()";
                IdUsuario = Convert.ToInt32(cmd.ExecuteScalar());

                // Guardar el ID en PlayerPrefs para futuras sesiones
                PlayerPrefs.SetInt("IdUsuario", IdUsuario);
                PlayerPrefs.Save();

                // Asignar el nombre a la propiedad pública
                NombreUsuario = nombre;
            }

            // Buscar instancia de DBManager para crear datasaves asociados a este usuario
            DBManager dbManager = FindAnyObjectByType<DBManager>();
            if (dbManager != null)
            {
                dbManager.CrearDatasavesSiNoExisten(IdUsuario);
            }
            else
            {
                Debug.LogError("No se encontró una instancia de DBManager en la escena.");
            }
        }
    }
}

