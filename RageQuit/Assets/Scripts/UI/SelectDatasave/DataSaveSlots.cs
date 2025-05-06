using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mono.Data.Sqlite;
using TMPro;

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

        // Cambia de escena a selección de niveles
        UnityEngine.SceneManagement.SceneManager.LoadScene("LevelSelectionScene");
    }
}
