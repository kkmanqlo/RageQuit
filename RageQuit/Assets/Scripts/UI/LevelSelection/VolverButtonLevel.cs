using UnityEngine;
using UnityEngine.SceneManagement;
public class VolverButtonLevel : MonoBehaviour
{
    public void VolverAEscenaAnterior()
    {
        SceneManager.LoadScene("DataSaveSelectionScene"); 
    }
}
