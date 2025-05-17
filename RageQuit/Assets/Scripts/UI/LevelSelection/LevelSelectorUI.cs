using UnityEngine;
using UnityEngine.UI;
using Mono.Data.Sqlite;
using System.Collections.Generic;
using TMPro;


public class LevelSelectorUI : MonoBehaviour
{
    public Transform container;                 // Contenedor para los botones de nivel
    public GameObject levelButtonPrefab;        // Prefab del botón de nivel
    private string dbPath;                       // Ruta a la base de datos SQLite
    private GameObject popupActivo;              // Referencia al popup actualmente activo

    public GameObject popupGeneral;              // Popup general para mostrar info del nivel

    void OnEnable()
    {
        // Validar que el contenedor esté asignado
        if (container == null)
        {
            Debug.LogError("El contenedor de botones no está asignado en el inspector.");
            return;
        }

        // Definir ruta a la base de datos
        dbPath = "URI=file:" + Application.persistentDataPath + "/RageQuitDB.db";

        // Mostrar los niveles desbloqueados
        MostrarNivelesDesbloqueados();
    }

    void MostrarNivelesDesbloqueados()
    {
        // Limpiar botones previos en el contenedor
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }

        int idProgreso = GameSession.Instance.IdProgreso;
        int nivelActual = 0;

        using (var conexion = new SqliteConnection(dbPath))
        {
            conexion.Open();

            // Obtener nivel actual del progreso del jugador
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
                        return; // Salir si no hay progreso válido
                    }
                }
            }

            // Obtener todos los niveles ordenados por id
            using (var cmd = conexion.CreateCommand())
            {
                cmd.CommandText = "SELECT idNivel, nombreNivel FROM Niveles ORDER BY idNivel ASC";

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int idNivel = reader.GetInt32(0);
                        string nombre = reader.GetString(1);

                        // Solo crear botón para niveles desbloqueados (id <= nivel actual)
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
        // Instanciar botón dentro del contenedor
        GameObject obj = Instantiate(levelButtonPrefab, container);

        // Asignar texto con el nombre del nivel
        obj.transform.Find("NombreNivel").GetComponent<TextMeshProUGUI>().text = nombreNivel;

        // Obtener el componente Button y asignar evento click
        Button boton = obj.GetComponentInChildren<Button>();

        // Capturar idNivel para usar dentro del lambda
        int capturedIdNivel = idNivel;

        // Agregar listener para mostrar popup al hacer click en el botón
        boton.onClick.AddListener(() => MostrarPopupNivel(obj, capturedIdNivel, nombreNivel, idProgreso));
    }

    void MostrarPopupNivel(GameObject botonNivel, int idNivel, string nombreNivel, int idProgreso)
    {
        // Ocultar popup activo previo, si hay
        if (popupActivo != null)
            popupActivo.SetActive(false);

        Transform popup = popupGeneral.transform;

        string textoStats = $"Level: {nombreNivel}\n";

        // Si es tutorial, indicamos que no se registran estadísticas
        if (idNivel == 1)
        {
            textoStats += "\nThis is a tutorial,\n";
            textoStats += "stats will not be recorded.";
        }
        else
        {
            // Consultar estadísticas guardadas en base de datos
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

        // Actualizar texto del popup con las estadísticas o mensaje
        popup.Find("TextoStats").GetComponent<TextMeshProUGUI>().text = textoStats;

        // Mostrar popup
        popup.gameObject.SetActive(true);
        popupActivo = popup.gameObject;

        // Configurar botón jugar con evento para cargar nivel
        Button jugarBtn = popup.Find("BotonJugar").GetComponent<Button>();
        jugarBtn.onClick.RemoveAllListeners();
        jugarBtn.onClick.AddListener(() => CargarNivel(nombreNivel));

        // Configurar botón cancelar para ocultar popup
        Button closeBtn = popup.Find("BotonCancelar").GetComponent<Button>();
        closeBtn.onClick.RemoveAllListeners();
        closeBtn.onClick.AddListener(() => OcultarPopUp());
    }

    void CargarNivel(string nombreEscena)
    {
        // Cargar escena según el nombre recibido
        UnityEngine.SceneManagement.SceneManager.LoadScene(nombreEscena);
    }

    public void OcultarPopUp()
    {
        // Ocultar popup activo si existe
        if (popupActivo != null)
            popupActivo.SetActive(false);
    }
}


