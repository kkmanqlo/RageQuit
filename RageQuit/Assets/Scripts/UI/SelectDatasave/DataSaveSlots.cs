using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DataSaveSlots : MonoBehaviour
{
   public Button slot1Button;
    public Button slot2Button;
    public Button slot3Button;

    private void Start()
    {
        // Asignar acciones a cada botón
        slot1Button.onClick.AddListener(() => SeleccionarSlot(1));
        slot2Button.onClick.AddListener(() => SeleccionarSlot(2));
        slot3Button.onClick.AddListener(() => SeleccionarSlot(3));
    }

    // Método que se llama al seleccionar un slot
    private void SeleccionarSlot(int slot)
    {
        // Guardamos el idProgreso correspondiente al slot
        GameSession.idProgreso = slot;  // O usa un valor que identifique al slot de manera única
        // Cargamos la siguiente escena (por ejemplo, la selección de niveles)
        SceneManager.LoadScene("SelectorNiveles");
    }
}
