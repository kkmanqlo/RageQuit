using UnityEngine;
using UnityEngine.SceneManagement;
public class VolverButton : MonoBehaviour
{
   public static string escenaAnterior = "";

    public void VolverAEscenaAnterior()
    {
        if (!string.IsNullOrEmpty(escenaAnterior))
        {
            SceneManager.LoadScene(escenaAnterior);
        }
        else
        {
            Debug.LogWarning("No hay escena anterior registrada.");
        }
    }
}
