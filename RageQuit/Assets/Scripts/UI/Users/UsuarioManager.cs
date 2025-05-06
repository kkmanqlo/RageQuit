using UnityEngine;
using Mono.Data.Sqlite;
using System;

public class UsuarioManager : MonoBehaviour
{
    public static UsuarioManager Instance;

    public string NombreUsuario { get; private set; }
    public int IdUsuario { get; private set; }

    private string dbPath;

    void Awake()
    {
        // Singleton
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

    public bool HayUsuarioRegistrado()
    {
        return PlayerPrefs.HasKey("IdUsuario");
    }

    public void CargarUsuario()
    {
        if (HayUsuarioRegistrado())
        {
            IdUsuario = PlayerPrefs.GetInt("IdUsuario");
            // Buscar el nombre en la DB
            using (var conexion = new SqliteConnection(dbPath))
            {
                conexion.Open();
                using (var cmd = conexion.CreateCommand())
                {
                    cmd.CommandText = "SELECT nombre FROM Usuarios WHERE idUsuario = @id";
                    cmd.Parameters.AddWithValue("@id", IdUsuario);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            NombreUsuario = reader.GetString(0);
                        }
                    }
                }
            }
        }
    }

    public void RegistrarNuevoUsuario(string nombre)
{
    using (var conexion = new SqliteConnection(dbPath))
    {
        conexion.Open();
        using (var cmd = conexion.CreateCommand())
        {
            cmd.CommandText = "INSERT INTO Usuarios (nombre, fechaRegistro) VALUES (@nombre, @fecha)";
            cmd.Parameters.AddWithValue("@nombre", nombre);
            cmd.Parameters.AddWithValue("@fecha", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.ExecuteNonQuery();

            // Obtener el ID del usuario insertado
            cmd.CommandText = "SELECT last_insert_rowid()";
            IdUsuario = Convert.ToInt32(cmd.ExecuteScalar());

            // Guardar en PlayerPrefs
            PlayerPrefs.SetInt("IdUsuario", IdUsuario);
            PlayerPrefs.Save();

            NombreUsuario = nombre;
        }

        
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
