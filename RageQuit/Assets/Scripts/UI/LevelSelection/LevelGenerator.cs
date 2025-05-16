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
                    
                    
                    InsertarNivel(cmd, "Tutorial", "Fácil");
                    InsertarNivel(cmd, "Level 1", "Fácil");
                    InsertarNivel(cmd, "Level 2", "Fácil");
                    InsertarNivel(cmd, "Level 3", "Medio");
                    InsertarNivel(cmd, "Level 4", "Difícil");
                    InsertarNivel(cmd, "Level 5", "Difícil");

                    
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
