using UnityEngine;
using UnityEngine.SceneManagement;

public class CambioDeEscena : MonoBehaviour
{
    public void CargarEscena(string nombreEscena)
    {
        VolverButton.escenaAnterior = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(nombreEscena);
    }
}
