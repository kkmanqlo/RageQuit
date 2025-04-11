using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GlowOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Outline glowEffect;

    void Start()
    {
        if (glowEffect != null)
            glowEffect.enabled = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (glowEffect != null)
            glowEffect.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (glowEffect != null)
            glowEffect.enabled = false;
    }
}
