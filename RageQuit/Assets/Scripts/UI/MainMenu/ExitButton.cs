using UnityEngine;

public class ExitButton : MonoBehaviour
{
    // Este script se encarga de manejar el botón de salir del juego en el menú principal.
    public void Exit()
    {
        Application.Quit();
        Debug.Log("Saliendo del juego...");
    }
}
