using UnityEngine;
using UnityEngine.UI;
using Mono.Data.Sqlite;
using System.Collections.Generic;
using TMPro;


public class LevelSelectorUI : MonoBehaviour
{
    public Transform container;
    public GameObject levelButtonPrefab;
    private string dbPath;
    private GameObject popupActivo;

    public GameObject popupGeneral;

    void OnEnable()
    {
        if (container == null)
        {
            Debug.LogError("El contenedor de botones no está asignado en el inspector.");
            return;
        }

        dbPath = "URI=file:" + Application.persistentDataPath + "/RageQuitDB.db";
        MostrarNivelesDesbloqueados();
    }

    void MostrarNivelesDesbloqueados()
    {
        // Limpiar botones anteriores
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }

        int idProgreso = GameSession.Instance.IdProgreso;
        int nivelActual = 0;

        using (var conexion = new SqliteConnection(dbPath))
        {
            conexion.Open();

            // Obtener nivel actual
            using (var cmd = conexion.CreateCommand())
            {

                cmd.CommandText = "SELECT nivelActual FROM ProgresoJugador WHERE idProgreso = @id";
                cmd.Parameters.AddWithValue("@id", idProgreso);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                       
                        nivelActual = reader.GetInt32(0);
                        
                    }
                    else
                    {
                        Debug.LogError("Error: No se encontró el nivel actual en la base de datos. No se mostrarán niveles.");
                        return; // Salimos si no hay progreso válido
                    }

                }
            }

            // Obtener niveles desbloqueados
            using (var cmd = conexion.CreateCommand())
            {
                cmd.CommandText = "SELECT idNivel, nombreNivel FROM Niveles ORDER BY idNivel ASC";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int idNivel = reader.GetInt32(0);
                        string nombre = reader.GetString(1);
                       
                        if (idNivel <= nivelActual)
                        {
                            
                            CrearBotonNivel(idNivel, nombre, idProgreso);
                        }
                    }
                }
            }
        }
    }

    void CrearBotonNivel(int idNivel, string nombreNivel, int idProgreso)
    {
        
        GameObject obj = Instantiate(levelButtonPrefab, container);
        obj.transform.Find("NombreNivel").GetComponent<TextMeshProUGUI>().text = nombreNivel;

        Button boton = obj.GetComponentInChildren<Button>();

        int capturedIdNivel = idNivel;
        boton.onClick.AddListener(() => MostrarPopupNivel(obj, capturedIdNivel, nombreNivel, idProgreso));

    }

    void MostrarPopupNivel(GameObject botonNivel, int idNivel, string nombreNivel, int idProgreso)
    {
        // Oculta cualquier popup previo
        if (popupActivo != null)
            popupActivo.SetActive(false);

        Transform popup = popupGeneral.transform;

        string textoStats = $"Level: {nombreNivel}\n";

        if (idNivel == 1)
        {
            textoStats += "\nThis is a tutorial,\n";
            textoStats += "stats will not be recorded.";
        }
        else
        {
            using (var conexion = new SqliteConnection(dbPath))
            {
                conexion.Open();
                using (var cmd = conexion.CreateCommand())
                {
                    cmd.CommandText = @"
                SELECT muertes, mejorTiempo 
                FROM EstadisticasNivel 
                WHERE idNivel = @nivel AND idProgreso = @progreso";
                    cmd.Parameters.AddWithValue("@nivel", idNivel);
                    cmd.Parameters.AddWithValue("@progreso", idProgreso);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            textoStats += $"\nDeaths: {reader.GetInt32(0)}\n";
                            textoStats += $"\nBest time: {reader.GetFloat(1):F2}s";
                        }
                        else
                        {
                            textoStats += "\nNo stats available for this level.";
                        }
                    }
                }
            }
        }

        popup.Find("TextoStats").GetComponent<TextMeshProUGUI>().text = textoStats;
        popup.gameObject.SetActive(true);
        popupActivo = popup.gameObject;

        Button jugarBtn = popup.Find("BotonJugar").GetComponent<Button>();
        jugarBtn.onClick.RemoveAllListeners();
        jugarBtn.onClick.AddListener(() => CargarNivel(nombreNivel));

        Button closeBtn = popup.Find("BotonCancelar").GetComponent<Button>();
        closeBtn.onClick.RemoveAllListeners();
        closeBtn.onClick.AddListener(() => OcultarPopUp());

    }

    void CargarNivel(string nombreEscena)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(nombreEscena);
    }

    public void OcultarPopUp()
    {
        popupActivo.SetActive(false);
    }

}

