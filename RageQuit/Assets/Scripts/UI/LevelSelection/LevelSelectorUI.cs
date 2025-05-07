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
                        Debug.Log("Se leyó correctamente el nivel actual.");
                        nivelActual = reader.GetInt32(0);
                        Debug.Log($"Nivel actual: {nivelActual}");
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
                        Debug.Log($"Nivel encontrado (segunda vez): ID = {idNivel}, Nombre = {nombre}");
                        if (idNivel <= nivelActual)
                        {
                            Debug.Log($"Creando botón (segunda vez) para el nivel: {nombre}");
                            CrearBotonNivel(idNivel, nombre, idProgreso);
                        }
                    }
                }
            }
        }
    }

    void CrearBotonNivel(int idNivel, string nombreNivel, int idProgreso)
    {
        Debug.Log($"Creando botón para el nivel: {nombreNivel}");
        Debug.Log($"Valor de levelButtonPrefab antes de Instantiate: {levelButtonPrefab}"); // Añade esta línea
        GameObject obj = Instantiate(levelButtonPrefab, container);
        obj.transform.Find("NombreNivel").GetComponent<TextMeshProUGUI>().text = nombreNivel;

        Button boton = obj.GetComponentInChildren<Button>();
        boton.onClick.AddListener(() => MostrarPopupNivel(obj, idNivel, nombreNivel, idProgreso));
    }

    void MostrarPopupNivel(GameObject botonNivel, int idNivel, string nombreNivel, int idProgreso)
    {
        // Oculta cualquier popup previo
        if (popupActivo != null)
            popupActivo.SetActive(false);

        Transform popup = botonNivel.transform.Find("Popup")
                 ?? botonNivel.transform.Find("Button/Popup")
                 ?? botonNivel.transform.GetComponentInChildren<Transform>(true).Find("Popup");

        string textoStats = $"Nivel: {nombreNivel}\n";

        using (var conexion = new SqliteConnection(dbPath))
        {
            conexion.Open();
            using (var cmd = conexion.CreateCommand())
            {
                cmd.CommandText = @"
                    SELECT muertes, tiempo, mejorTiempo 
                    FROM EstadisticasNivel 
                    WHERE idNivel = @nivel AND idProgreso = @progreso";
                cmd.Parameters.AddWithValue("@nivel", idNivel);
                cmd.Parameters.AddWithValue("@progreso", idProgreso);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        textoStats += $"Muertes: {reader.GetInt32(0)}\n";
                        textoStats += $"Tiempo: {reader.GetFloat(1):F2}s\n";
                        textoStats += $"Mejor Tiempo: {reader.GetFloat(2):F2}s";
                    }
                    else
                    {
                        textoStats += "Sin estadísticas aún.";
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

