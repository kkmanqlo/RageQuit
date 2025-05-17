using UnityEngine;

public class PlayerPrefsDelete : MonoBehaviour
{
    // Este script se encarga de borrar los PlayerPrefs de la aplicación.
    // Se utiliza principalmente para propósitos de desarrollo y depuración.

    // Método que se llama al hacer clic en el botón "Borrar PlayerPrefs".
    // Borra todos los PlayerPrefs guardados en la aplicación.
    [ContextMenu("Borrar PlayerPrefs")]
public void BorrarPrefs()
{
    PlayerPrefs.DeleteAll();
    Debug.Log("PlayerPrefs borrados.");
}
}
