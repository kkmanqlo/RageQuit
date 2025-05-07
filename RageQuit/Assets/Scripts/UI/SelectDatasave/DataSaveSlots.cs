using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mono.Data.Sqlite;
using TMPro;
using System;

public class DataSaveSlots : MonoBehaviour
{
    public Button[] slotButtons; // Asigna en el inspector (debe tener 3)
    private string dbPath;

    void Start()
    {
        dbPath = "URI=file:" + Application.persistentDataPath + "/RageQuitDB.db";
        CargarSlots();
    }

    void CargarSlots()
    {
        int idUsuario = UsuarioManager.Instance.IdUsuario;

        using (var conexion = new SqliteConnection(dbPath))
        {
            conexion.Open();
            using (var cmd = conexion.CreateCommand())
            {
                for (int i = 1; i <= 3; i++)
                {
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

                            string texto = $"Slot {i} – Nivel: {nivel} – Muertes: {muertes}";
                            slotButtons[i - 1].GetComponentInChildren<TextMeshProUGUI>().text = texto;


                            int slotIndex = i; // captura el valor para el closure del listener
                            slotButtons[i - 1].onClick.AddListener(() => SeleccionarSlot(slotIndex));
                        }
                    }
                }
            }
        }
    }

    void SeleccionarSlot(int slot)
    {
        GameSession.Instance.SlotSeleccionado = slot;
        Debug.Log("Slot seleccionado: " + slot);

        int idUsuario = UsuarioManager.Instance.IdUsuario;

        using (var conexion = new SqliteConnection(dbPath))
        {
            conexion.Open();

            using (var cmd = conexion.CreateCommand())
            {
                // Comprobar si ya existe ese progreso
                cmd.CommandText = "SELECT COUNT(*) FROM ProgresoJugador WHERE idUsuario = @id AND slotNumero = @slot";
                cmd.Parameters.AddWithValue("@id", idUsuario);
                cmd.Parameters.AddWithValue("@slot", slot);

                int count = Convert.ToInt32(cmd.ExecuteScalar());

                if (count == 0)
                {
                    Debug.Log("Insertando nuevo progreso con nivelActual = 1");

                    cmd.CommandText = @"
                    INSERT INTO ProgresoJugador (idUsuario, slotNumero, nivelActual, tiempoTotal, muertesTotales)
                    VALUES (@id, @slot, 1, 0, 0)";
                    cmd.ExecuteNonQuery();
                }

                // Obtener el idProgreso recién creado o existente
                cmd.CommandText = "SELECT idProgreso FROM ProgresoJugador WHERE idUsuario = @id AND slotNumero = @slot";
                int idProgreso = Convert.ToInt32(cmd.ExecuteScalar());
                GameSession.Instance.IdProgreso = idProgreso;
            }
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene("LevelSelectionScene");
    }
}
