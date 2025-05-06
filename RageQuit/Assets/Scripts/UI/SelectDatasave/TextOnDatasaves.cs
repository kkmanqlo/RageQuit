using UnityEngine;
using Mono.Data.Sqlite;
using TMPro;
using UnityEngine.UI;

public class TextOnDatasaves : MonoBehaviour
{
    private string dbPath;

    // Referencias a los botones y sus TextMeshPro
    public Button button1;
    public Button button2;
    public Button button3;

    // Referencias a los TextMeshPro de cada botón
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
        dbPath = "URI=file:" + Application.persistentDataPath + "/RageQuitDB.db";
        CargarDatos();
    }

    void CargarDatos()
    {
        int idUsuario = UsuarioManager.Instance.IdUsuario;; 

        // Datos predeterminados en caso de que los valores sean 0
        string ultimoNivel = "No hay niveles completados";
        string tiempoJugado = "No hay tiempo jugado";
        string muertesTotales = "No hay muertes registradas";

        using (var conexion = new SqliteConnection(dbPath))
        {
            conexion.Open();
            using (var cmd = conexion.CreateCommand())
            {
                // Consulta para obtener el progreso del jugador
                cmd.CommandText = @"
                SELECT nivelActual, tiempoTotal, muertesTotales
                FROM ProgresoJugador
                WHERE idUsuario = @idUsuario
                LIMIT 1;
            ";
                cmd.Parameters.AddWithValue("@idUsuario", idUsuario);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Recuperar los valores de la base de datos
                        int nivelActual = reader.GetInt32(0);
                        float tiempoTotal = reader.GetFloat(1);
                        int muertes = reader.GetInt32(2);

                        // Comprobar si los valores son 0 y, si lo son, usar los textos predeterminados
                        if (nivelActual > 0)
                        {
                            ultimoNivel = "Último Nivel completado: " + nivelActual;
                        }

                        if (tiempoTotal > 0)
                        {
                            tiempoJugado = "Tiempo Jugado: " + tiempoTotal + " horas";
                        }

                        if (muertes > 0)
                        {
                            muertesTotales = "Muertes Totales: " + muertes;
                        }
                    }
                }
            }
        }

        // Asignar los datos a los TextMeshPro de los tres botones
        AsignarDatosABoton(button1NivelText, button1TiempoText, button1MuertesText, ultimoNivel, tiempoJugado, muertesTotales);
        AsignarDatosABoton(button2NivelText, button2TiempoText, button2MuertesText, ultimoNivel, tiempoJugado, muertesTotales);
        AsignarDatosABoton(button3NivelText, button3TiempoText, button3MuertesText, ultimoNivel, tiempoJugado, muertesTotales);
    }


    void AsignarDatosABoton(TextMeshProUGUI nivelText, TextMeshProUGUI tiempoText, TextMeshProUGUI muertesText, string ultimoNivel, string tiempoJugado, string muertesTotales)
    {
        nivelText.text = ultimoNivel;
        tiempoText.text = tiempoJugado;
        muertesText.text = muertesTotales;
    }
}