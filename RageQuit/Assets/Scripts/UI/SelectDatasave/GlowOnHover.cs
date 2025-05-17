using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GlowOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Referencia al componente Outline que genera el efecto de resplandor (glow)
    public Outline glowEffect;

    void Start()
    {
        // Asegurarse de que el efecto glow esté desactivado al iniciar
        if (glowEffect != null)
            glowEffect.enabled = false;
    }

    // Método que se llama cuando el puntero (mouse) entra sobre el elemento UI
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (glowEffect != null)
            glowEffect.enabled = true; // Activar el efecto glow
    }

    // Método que se llama cuando el puntero sale del elemento UI
    public void OnPointerExit(PointerEventData eventData)
    {
        if (glowEffect != null)
            glowEffect.enabled = false; // Desactivar el efecto glow
    }
}

