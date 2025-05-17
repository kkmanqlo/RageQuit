using UnityEngine;
using UnityEngine.SceneManagement;
public class VolverButton : MonoBehaviour
{
    // Este script se encarga de manejar el botón de volver a la escena anterior en la selección de datos guardados.
    // Al hacer clic en el botón, se carga la escena "MenuPrincipal".
    public void VolverAEscenaAnterior()
    {
        SceneManager.LoadScene("MenuPrincipal"); 
    }
}
