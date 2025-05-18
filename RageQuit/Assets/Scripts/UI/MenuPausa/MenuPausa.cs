using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPausa : MonoBehaviour
{

    public static bool menuBloqueado = false;

    [SerializeField] private GameObject menuPausa; // Referencia al GameObject que contiene el menú de pausa
    private bool isPaused = false; // Controla si el juego está pausado o no

    void Update()
    {
        if (menuBloqueado) return;

        if (GameManager.Instance.menuInputBloqueado) return;
        // Detecta si se presiona la tecla Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
            {
                Pausar();  // Si no está pausado, pausar el juego
            }
            else
            {
                Reanudar(); // Si está pausado, reanudar el juego
            }
        }
    }

    // Método para pausar el juego
    private void Pausar()
    {
        Time.timeScale = 0f;           // Congela el tiempo del juego
        menuPausa.SetActive(true);     // Muestra el menú de pausa
        isPaused = true;               // Actualiza el estado a pausado
        GameManager.Instance.inputBloqueado = true; // Bloquear input
    }

    // Método público para reanudar el juego desde el menú
    public void Reanudar()
    {
        Time.timeScale = 1f;           // Restaura el tiempo normal
        menuPausa.SetActive(false);    // Oculta el menú de pausa
        isPaused = false;              // Actualiza el estado a no pausado
        GameManager.Instance.inputBloqueado = false; // Bloquear input
    }

    // Método para salir al menú principal desde el menú de pausa
    public void SalirMenu()
    {
        SceneManager.LoadScene("MenuPrincipal"); // Carga la escena del menú principal
        menuPausa.SetActive(false);               // Oculta el menú de pausa
        isPaused = false;                         // Actualiza estado
        Time.timeScale = 1f;                      // Asegura que el tiempo esté normal
        DOTween.KillAll();                        // Detiene todas las animaciones de DOTween para evitar bugs
    }

    // Método para salir del juego
    public void Exit()
    {
        Application.Quit();            // Cierra la aplicación
        Debug.Log("Saliendo del juego..."); // Mensaje en consola (útil en editor)
    }
}

