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
        int idUsuario = UsuarioManager.Instance.IdUsuario;

        using (var conexion = new SqliteConnection(dbPath))
        {
            conexion.Open();
            using (var cmd = conexion.CreateCommand())
            {
                for (int i = 1; i <= 3; i++)
                {
                    // Reiniciar textos predeterminados por slot
                    string ultimoNivel = "No hay niveles completados";
                    string tiempoJugado = "No hay tiempo jugado";
                    string muertesTotales = "No hay muertes registradas";

                    // Consulta para obtener el progreso del jugador y el nombre del nivel para cada slot
                    cmd.CommandText = @"
                    SELECT p.nivelActual, n.nombreNivel, p.tiempoTotal, p.muertesTotales
                    FROM ProgresoJugador p
                    INNER JOIN Niveles n ON p.nivelActual = n.idNivel
                    WHERE p.idUsuario = @idUsuario AND p.slotNumero = @slot
                    LIMIT 1;
                ";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@idUsuario", idUsuario);
                    cmd.Parameters.AddWithValue("@slot", i);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int nivelActual = reader.GetInt32(0);
                            string nombreNivel = reader.GetString(1);
                            float tiempoTotal = reader.GetFloat(2);
                            int muertes = reader.GetInt32(3);

                            if (nivelActual > 0)
                                ultimoNivel = "Nivel Actual: " + nombreNivel;

                            if (tiempoTotal > 0)
                                tiempoJugado = "Tiempo Jugado: " + tiempoTotal + " horas";

                            if (muertes > 0)
                                muertesTotales = "Muertes Totales: " + muertes;
                        }
                    }

                    // Asignar los datos al botón correspondiente
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


    void AsignarDatosABoton(TextMeshProUGUI nivelText, TextMeshProUGUI tiempoText, TextMeshProUGUI muertesText, string ultimoNivel, string tiempoJugado, string muertesTotales)
    {
        nivelText.text = ultimoNivel;
        tiempoText.text = tiempoJugado;
        muertesText.text = muertesTotales;
    }
}
