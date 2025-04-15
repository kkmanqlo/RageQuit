using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ControladorMenu : MonoBehaviour
{
    public GameObject PopUp;
    public TMP_InputField inputNombre;
    public TMP_Text textoNombreVisible;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (UsuarioManager.Instance.HayUsuarioRegistrado())
        {
            UsuarioManager.Instance.CargarUsuario();
            MostrarNombreEnPantalla();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void BotonJugar()
    {
        if (UsuarioManager.Instance.HayUsuarioRegistrado())
        {
            VolverButton.escenaAnterior = SceneManager.GetActiveScene().name;
            // Ir directamente al juego
            SceneManager.LoadScene("DataSaveSelectionScene");
        }
        else
        {
            PopUp.SetActive(true);
        }
    }

    public void ConfirmarNombre()
    {
        string nombre = inputNombre.text.Trim();
        if (nombre != "")
        {
            UsuarioManager.Instance.RegistrarNuevoUsuario(nombre);
            PopUp.SetActive(false);
            MostrarNombreEnPantalla();
        }
    }

    public void OcultarPopUp()
    {
        PopUp.SetActive(false);
    }

    void MostrarNombreEnPantalla()
    {
        textoNombreVisible.text = UsuarioManager.Instance.NombreUsuario;
        textoNombreVisible.gameObject.SetActive(true);
    }
}
