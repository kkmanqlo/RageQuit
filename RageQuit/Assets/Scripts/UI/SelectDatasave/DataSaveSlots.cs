using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mono.Data.Sqlite;
using TMPro;
using System;

public class DataSaveSlots : MonoBehaviour
{
    public Button[] slotButtons; // Array de botones para los slots (deben ser 3 y asignados en inspector)
    private string dbPath;       // Ruta a la base de datos SQLite

    void Start()
    {
        // Construye la ruta a la base de datos local
        dbPath = "URI=file:" + Application.persistentDataPath + "/RageQuitDB.db";
        // Carga y muestra la info de cada slot en los botones
        CargarSlots();
    }

    void CargarSlots()
    {
        int idUsuario = UsuarioManager.Instance.IdUsuario; // ID del usuario actual

        using (var conexion = new SqliteConnection(dbPath))
        {
            conexion.Open();
            using (var cmd = conexion.CreateCommand())
            {
                // Itera por cada uno de los 3 slots
                for (int i = 1; i <= 3; i++)
                {
                    // Consulta el progreso guardado para el usuario en ese slot
                    cmd.CommandText = @"
                        SELECT nivelActual, tiempoTotal, muertesTotales 
                        FROM ProgresoJugador 
                        WHERE idUsuario = @id AND slotNumero = @slot";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@id", idUsuario);
                    cmd.Parameters.AddWithValue("@slot", i);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int nivel = reader.GetInt32(0);
                            float tiempo = reader.GetFloat(1);
                            int muertes = reader.GetInt32(2);

                            // Muestra la info en el texto del botón correspondiente
                            string texto = $"Slot {i} – Nivel: {nivel} – Muertes: {muertes}";
                            slotButtons[i - 1].GetComponentInChildren<TextMeshProUGUI>().text = texto;

                            // Añade el listener para seleccionar ese slot
                            int slotIndex = i; // Captura el valor para el cierre del listener
                            slotButtons[i - 1].onClick.AddListener(() => SeleccionarSlot(slotIndex));
                        }
                    }
                }
            }
        }
    }

    void SeleccionarSlot(int slot)
    {
        // Guarda el slot seleccionado en la sesión actual
        GameSession.Instance.SlotSeleccionado = slot;
        Debug.Log("Slot seleccionado: " + slot);

        int idUsuario = UsuarioManager.Instance.IdUsuario;

        using (var conexion = new SqliteConnection(dbPath))
        {
            conexion.Open();

            using (var cmd = conexion.CreateCommand())
            {
                // Comprueba si existe un progreso para ese usuario y slot
                cmd.CommandText = "SELECT COUNT(*) FROM ProgresoJugador WHERE idUsuario = @id AND slotNumero = @slot";
                cmd.Parameters.AddWithValue("@id", idUsuario);
                cmd.Parameters.AddWithValue("@slot", slot);

                int count = Convert.ToInt32(cmd.ExecuteScalar());

                if (count == 0)
                {
                    // Si no existe progreso, inserta un nuevo registro con nivel 1 y stats en 0
                    Debug.Log("Insertando nuevo progreso con nivelActual = 1");

                    cmd.CommandText = @"
                    INSERT INTO ProgresoJugador (idUsuario, slotNumero, nivelActual, tiempoTotal, muertesTotales)
                    VALUES (@id, @slot, 1, 0, 0)";
                    cmd.ExecuteNonQuery();
                }

                // Obtiene el idProgreso del progreso existente o recién creado
                cmd.CommandText = "SELECT idProgreso FROM ProgresoJugador WHERE idUsuario = @id AND slotNumero = @slot";
                int idProgreso = Convert.ToInt32(cmd.ExecuteScalar());

                // Guarda el progreso seleccionado en la sesión
                GameSession.Instance.IdProgreso = idProgreso;          
            }
        }

        // Carga la escena para seleccionar el nivel
        UnityEngine.SceneManagement.SceneManager.LoadScene("LevelSelectionScene");
    }
}

