using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ControladorMenu : MonoBehaviour
{
    public GameObject PopUp;                   // Referencia al popup para ingresar nombre
    public TMP_InputField inputNombre;        // Campo de texto donde el usuario escribe su nombre
    public TMP_Text textoNombreVisible;       // Texto donde se muestra el nombre del usuario en pantalla

    // Se ejecuta una vez al iniciar la escena
    void Start()
    {
        // Si ya hay un usuario registrado, cargar sus datos y mostrar el nombre en pantalla
        if (UsuarioManager.Instance.HayUsuarioRegistrado())
        {
            UsuarioManager.Instance.CargarUsuario();
            MostrarNombreEnPantalla();
        }
    }

    // Método que se llama al presionar el botón "Jugar"
    public void BotonJugar()
    {
        // Si ya existe un usuario registrado, cargar la escena de selección de datasave
        if (UsuarioManager.Instance.HayUsuarioRegistrado())
        {
            SceneManager.LoadScene("DataSaveSelectionScene");
        }
        else
        {
            // Si no hay usuario, mostrar el popup para registrar uno nuevo
            PopUp.SetActive(true);
        }
    }

    // Método que confirma el nombre ingresado en el popup
    public void ConfirmarNombre()
    {
        // Obtener el texto ingresado y eliminar espacios en blanco al inicio y final
        string nombre = inputNombre.text.Trim();

        // Si el nombre no está vacío
        if (nombre != "")
        {
            // Registrar el nuevo usuario con ese nombre
            UsuarioManager.Instance.RegistrarNuevoUsuario(nombre);

            // Ocultar el popup
            PopUp.SetActive(false);

            // Mostrar el nombre en la interfaz
            MostrarNombreEnPantalla();
        }
    }

    // Método para ocultar el popup (por ejemplo, al cancelar)
    public void OcultarPopUp()
    {
        PopUp.SetActive(false);
    }

    // Muestra el nombre del usuario en la pantalla principal
    void MostrarNombreEnPantalla()
    {
        textoNombreVisible.text = UsuarioManager.Instance.NombreUsuario;
        textoNombreVisible.gameObject.SetActive(true);
    }
}

