using UnityEngine;
using Mono.Data.Sqlite;
using TMPro;
using UnityEngine.UI;
using System;

public class TextOnDatasaves : MonoBehaviour
{
    private string dbPath;

    // Referencias a los botones y sus textos (TextMeshPro) para mostrar la info de cada slot
    public Button button1;
    public Button button2;
    public Button button3;

    public TextMeshProUGUI button1NivelText;
    public TextMeshProUGUI button1TiempoText;
    public TextMeshProUGUI button1MuertesText;

    public TextMeshProUGUI button2NivelText;
    public TextMeshProUGUI button2TiempoText;
    public TextMeshProUGUI button2MuertesText;

    public TextMeshProUGUI button3NivelText;
    public TextMeshProUGUI button3TiempoText;
    public TextMeshProUGUI button3MuertesText;

    void Start()
    {
        // Ruta a la base de datos local
        dbPath = "URI=file:" + Application.persistentDataPath + "/RageQuitDB.db";
        CargarDatos();
    }

    // Carga los datos de progreso guardados y actualiza los textos en los botones
    void CargarDatos()
    {
        int idUsuario = UsuarioManager.Instance.IdUsuario;

        using (var conexion = new SqliteConnection(dbPath))
        {
            conexion.Open();

            // Obtener el ID del último nivel disponible para validar progresos
            int ultimoNivelID = 0;
            using (var cmdMax = conexion.CreateCommand())
            {
                cmdMax.CommandText = "SELECT MAX(idNivel) FROM Niveles;";
                var result = cmdMax.ExecuteScalar();
                if (result != DBNull.Value)
                    ultimoNivelID = Convert.ToInt32(result);
            }

            using (var cmd = conexion.CreateCommand())
            {
                // Para cada slot de progreso (1 a 3) consulta y muestra los datos
                for (int i = 1; i <= 3; i++)
                {
                    // Valores por defecto si no hay datos
                    string ultimoNivel = "No progress detected";
                    string tiempoJugado = "No time played";
                    string muertesTotales = "No deaths recorded";
                    string tiempoFormateado = "No time played";

                    // Consulta el progreso del jugador para el slot i
                    cmd.CommandText = @"
                    SELECT nivelActual, tiempoTotal, muertesTotales
                    FROM ProgresoJugador
                    WHERE idUsuario = @idUsuario AND slotNumero = @slot
                    LIMIT 1;
                    ";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@idUsuario", idUsuario);
                    cmd.Parameters.AddWithValue("@slot", i);

                    int nivelActual = 0;
                    float tiempoTotal = 0;
                    int muertes = 0;

                    // Leer datos de la consulta
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            nivelActual = reader.GetInt32(0);
                            tiempoTotal = reader.GetFloat(1);
                            muertes = reader.GetInt32(2);
                        }
                    }

                    // Buscar nombre del nivel actual solo si existe dentro de los niveles definidos
                    string nombreNivel = null;
                    if (nivelActual > 0 && nivelActual <= ultimoNivelID)
                    {
                        using (var cmdNivel = conexion.CreateCommand())
                        {
                            cmdNivel.CommandText = "SELECT nombreNivel FROM Niveles WHERE idNivel = @nivelID;";
                            cmdNivel.Parameters.AddWithValue("@nivelID", nivelActual);

                            var result = cmdNivel.ExecuteScalar();
                            if (result != null)
                                nombreNivel = result.ToString();
                        }
                    }

                    // Evaluar el estado para mostrar en pantalla
                    if (nivelActual > 0)
                    {
                        if (nivelActual > ultimoNivelID)
                            ultimoNivel = "All levels completed!";
                        else if (!string.IsNullOrEmpty(nombreNivel))
                            ultimoNivel = "Current Level: " + nombreNivel;
                    }

                    // Formatear el tiempo total jugado
                    if (tiempoTotal > 0)
                    {
                        tiempoFormateado = System.TimeSpan.FromSeconds(tiempoTotal).ToString(@"hh\:mm\:ss");
                        tiempoJugado = "Time played: " + tiempoFormateado;
                    }

                    // Mostrar muertes totales si hay
                    if (muertes > 0)
                        muertesTotales = "Total deaths: " + muertes;

                    // Asignar la info al botón correspondiente
                    if (i == 1)
                        AsignarDatosABoton(button1NivelText, button1TiempoText, button1MuertesText, ultimoNivel, tiempoJugado, muertesTotales);
                    else if (i == 2)
                        AsignarDatosABoton(button2NivelText, button2TiempoText, button2MuertesText, ultimoNivel, tiempoJugado, muertesTotales);
                    else if (i == 3)
                        AsignarDatosABoton(button3NivelText, button3TiempoText, button3MuertesText, ultimoNivel, tiempoJugado, muertesTotales);
                }
            }
        }
    }

    // Actualiza los textos de nivel, tiempo y muertes en el botón correspondiente
    void AsignarDatosABoton(TextMeshProUGUI nivelText, TextMeshProUGUI tiempoText, TextMeshProUGUI muertesText, string ultimoNivel, string tiempoJugado, string muertesTotales)
    {
        nivelText.text = ultimoNivel;
        tiempoText.text = tiempoJugado;
        muertesText.text = muertesTotales;
    }
}

