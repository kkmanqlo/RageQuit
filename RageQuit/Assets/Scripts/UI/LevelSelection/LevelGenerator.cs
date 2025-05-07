using UnityEngine;
using Mono.Data.Sqlite;
using System;

public class LevelGenerator : MonoBehaviour
{
    private string dbPath;

    void Start()
{
    if (!PlayerPrefs.HasKey("NivelesInsertados"))
    {
        dbPath = "URI=file:" + Application.persistentDataPath + "/RageQuitDB.db";
        InsertarNivelesSiNoExisten();
        PlayerPrefs.SetInt("NivelesInsertados", 1);
        PlayerPrefs.Save(); 
    }

    Destroy(gameObject);
}

    void InsertarNivelesSiNoExisten()
    {
        using (var conexion = new SqliteConnection(dbPath))
        {
            conexion.Open();
            using (var cmd = conexion.CreateCommand())
            {
                cmd.CommandText = "SELECT COUNT(*) FROM Niveles";
                int count = Convert.ToInt32(cmd.ExecuteScalar());

                if (count == 0)
                {
                    Debug.Log("Insertando niveles manualmente...");
                    
                    InsertarNivel(cmd, "Tutorial", "Fácil");
                    InsertarNivel(cmd, "Nivel 1", "Fácil");
                    InsertarNivel(cmd, "Nivel 2", "Fácil");
                    InsertarNivel(cmd, "Nivel 3", "Medio");
                    InsertarNivel(cmd, "Nivel 4", "Difícil");
                    InsertarNivel(cmd, "Nivel 5", "Difícil");
                    InsertarNivel(cmd, "Nivel 6", "Extremo");
                    InsertarNivel(cmd, "Nivel 7", "Extremo");

                    Debug.Log("Niveles insertados correctamente.");
                }
                else
                {
                    Debug.Log("Ya existen niveles en la base de datos.");
                }
            }
        }

        // Destruir el GameObject después de insertar los niveles
        Destroy(gameObject);
    }

    void InsertarNivel(SqliteCommand cmd, string nombre, string dificultad)
    {
        cmd.CommandText = @"
            INSERT INTO Niveles (nombreNivel, dificultad)
            VALUES (@nombre, @dificultad)";
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@nombre", nombre);
        cmd.Parameters.AddWithValue("@dificultad", dificultad);
        cmd.ExecuteNonQuery();
    }
}
