using UnityEngine;

public class OptionsController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static OptionsController instance;

    private void OnEnable()
    {
        MenuPausa.menuBloqueado = true; // Bloquea ESC mientras estás en opciones
    }

    private void OnDisable()
    {
        MenuPausa.menuBloqueado = false; // Desbloquea ESC al salir
    }
}
