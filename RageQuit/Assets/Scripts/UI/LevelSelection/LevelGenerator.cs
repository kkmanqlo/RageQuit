using UnityEngine;
using Mono.Data.Sqlite;
using System;

public class LevelGenerator : MonoBehaviour
{
    private string dbPath;

    void Start()
    {
        // Verifica si ya se insertaron los niveles en PlayerPrefs
        if (!PlayerPrefs.HasKey("NivelesInsertados"))
        {
            // Define la ruta de la base de datos SQLite
            dbPath = "URI=file:" + Application.persistentDataPath + "/RageQuitDB.db";

            // Inserta los niveles en la base de datos si no existen
            InsertarNivelesSiNoExisten();

            // Marca en PlayerPrefs que ya se insertaron los niveles
            PlayerPrefs.SetInt("NivelesInsertados", 1);
            PlayerPrefs.Save(); 
        }

        // Destruye el GameObject para no repetir esta acción
        Destroy(gameObject);
    }

    void InsertarNivelesSiNoExisten()
    {
        using (var conexion = new SqliteConnection(dbPath))
        {
            conexion.Open();
            using (var cmd = conexion.CreateCommand())
            {
                // Cuenta cuántos niveles hay en la tabla Niveles
                cmd.CommandText = "SELECT COUNT(*) FROM Niveles";
                int count = Convert.ToInt32(cmd.ExecuteScalar());

                // Si no hay niveles, los inserta
                if (count == 0)
                {
                    InsertarNivel(cmd, "Tutorial", "Fácil");
                    InsertarNivel(cmd, "Level 1", "Fácil");
                    InsertarNivel(cmd, "Level 2", "Fácil");
                    InsertarNivel(cmd, "Level 3", "Medio");
                    InsertarNivel(cmd, "Level 4", "Difícil");
                    InsertarNivel(cmd, "Level 5", "Difícil");
                }
            }
        }

        // Destruye el GameObject después de insertar los niveles (aunque ya se destruye en Start)
        Destroy(gameObject);
    }

    void InsertarNivel(SqliteCommand cmd, string nombre, string dificultad)
    {
        // Prepara la sentencia SQL para insertar un nivel con parámetros
        cmd.CommandText = @"
            INSERT INTO Niveles (nombreNivel, dificultad)
            VALUES (@nombre, @dificultad)";
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@nombre", nombre);
        cmd.Parameters.AddWithValue("@dificultad", dificultad);

        // Ejecuta la inserción en la base de datos
        cmd.ExecuteNonQuery();
    }
}
