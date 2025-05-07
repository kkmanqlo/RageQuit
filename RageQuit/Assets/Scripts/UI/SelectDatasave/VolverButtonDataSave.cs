using UnityEngine;
using UnityEngine.SceneManagement;
public class VolverButton : MonoBehaviour
{
    public void VolverAEscenaAnterior()
    {
        SceneManager.LoadScene("MenuPrincipal"); 
    }
}
