using UnityEngine;
using Mono.Data.Sqlite;
using System.IO;
using System.Data;

public class DBManager : MonoBehaviour
{
    private string dbPath;

    void Start()
    {
        dbPath = "URI=file:" + Application.persistentDataPath + "/RageQuitDB.db";
        CrearBaseDeDatos();
    }

    void CrearBaseDeDatos()
    {
        using (var conexion = new SqliteConnection(dbPath))
        {
            conexion.Open();
            using (var cmd = conexion.CreateCommand())
            {
                // Tabla Usuarios
                cmd.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Usuarios (
                        idUsuario INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                        nombre TEXT NOT NULL,
                        fechaRegistro DATETIME NOT NULL
                    );
                "; cmd.ExecuteNonQuery();

                // Tabla ProgresoJugador
                cmd.CommandText = @"
                    CREATE TABLE IF NOT EXISTS ProgresoJugador (
                    idProgreso INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                    idUsuario INTEGER NOT NULL,
                    slotNumero INTEGER NOT NULL CHECK (slotNumero IN (1, 2, 3)),
                    nivelActual INTEGER NOT NULL,
                    tiempoTotal REAL NOT NULL,
                    muertesTotales INTEGER NOT NULL,
                    FOREIGN KEY(idUsuario) REFERENCES Usuarios(idUsuario)
                    );
                "; cmd.ExecuteNonQuery();

                // Tabla Niveles
                cmd.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Niveles (
                        idNivel INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                        nombreNivel TEXT NOT NULL,
                        dificultad TEXT NOT NULL CHECK (dificultad IN ('Fácil', 'Medio', 'Difícil', 'Extremo'))
                    );
                "; cmd.ExecuteNonQuery();

                // Tabla EstadisticasNivel
                cmd.CommandText = @"
                    CREATE TABLE IF NOT EXISTS EstadisticasNivel (
                        idEstadistica INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                        idProgreso INTEGER NOT NULL,
                        idNivel INTEGER NOT NULL UNIQUE,
                        muertes INTEGER NOT NULL,
                        tiempo REAL NOT NULL,
                        mejorTiempo REAL NOT NULL,
                        FOREIGN KEY(idProgreso) REFERENCES ProgresoJugador(idProgreso),
                        FOREIGN KEY(idNivel) REFERENCES Niveles(idNivel)
                    );
                "; cmd.ExecuteNonQuery();
            }
        }

        Debug.Log("Base de datos creada en: " + dbPath);
    }

    public void CrearDatasavesSiNoExisten(int idUsuario)
    {
        using (var conexion = new SqliteConnection(dbPath))
        {
            conexion.Open();
            using (var cmd = conexion.CreateCommand())
            {
                for (int slot = 1; slot <= 3; slot++)
                {
                    cmd.CommandText = @"
                        INSERT INTO ProgresoJugador (idUsuario, slotNumero, nivelActual, tiempoTotal, muertesTotales)
                        SELECT @id, @slot, 1, 0.0, 0
                        WHERE NOT EXISTS (
                            SELECT 1 FROM ProgresoJugador 
                            WHERE idUsuario = @id AND slotNumero = @slot
                        );";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@id", idUsuario);
                    cmd.Parameters.AddWithValue("@slot", slot);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        Debug.Log("Datasaves creados si no existían para el usuario con ID: " + idUsuario);
    }
}
