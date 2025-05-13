using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPausa : MonoBehaviour
{
    [SerializeField] private GameObject menuPausa; // Referencia al menú de pausa
    private bool isPaused = false; // Variable para controlar el estado de pausa

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
            {
                Pausar();
            }
            else
            {
                Reanudar();
            }
        }
    }

    private void Pausar()
    {
        Time.timeScale = 0f;
        menuPausa.SetActive(true);
        isPaused = true;
    }

    public void Reanudar()
    {
        Time.timeScale = 1f;
        menuPausa.SetActive(false);
        isPaused = false;
    }

    public void SalirMenu()
    {

        SceneManager.LoadScene("MenuPrincipal");
        menuPausa.SetActive(false);
        isPaused = false;
        Time.timeScale = 1f;
        DOTween.KillAll(); // Detener todas las animaciones de DOTween
    }

    public void Exit()
    {
        Application.Quit();
        Debug.Log("Saliendo del juego...");
    }

}
