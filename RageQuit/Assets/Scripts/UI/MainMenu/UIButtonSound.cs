using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonSound : MonoBehaviour, IPointerEnterHandler
{
    // Este script se encarga de reproducir un sonido cuando el cursor entra en un botón o elemento UI.
    public void OnPointerEnter(PointerEventData eventData)
    {
        UIAudioManager.instance?.PlayHover();
    }

}
