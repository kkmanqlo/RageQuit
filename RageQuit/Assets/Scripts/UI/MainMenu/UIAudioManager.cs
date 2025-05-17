using UnityEngine;

public class UIAudioManager : MonoBehaviour
{
    public static UIAudioManager instance;  // Instancia estática para implementar el patrón Singleton

    public AudioClip hoverSound;             // Sonido que se reproduce cuando el cursor pasa sobre un botón o elemento UI

    private AudioSource audioSource;         // Componente AudioSource para reproducir sonidos

    void Awake()
    {
        // Si no existe una instancia previa, asignamos esta como la instancia única
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // No destruir este objeto al cambiar de escena
            audioSource = GetComponent<AudioSource>();  // Obtener el componente AudioSource del GameObject
        }
        else
        {
            // Si ya existe otra instancia, destruimos esta para mantener solo una
            Destroy(gameObject);
        }
    }

    // Método público para reproducir el sonido de "hover" (pasar el cursor)
    public void PlayHover()
    {
        if (hoverSound != null)              // Verificar que el clip de audio está asignado
            audioSource.PlayOneShot(hoverSound);  // Reproducir el sonido sin interferir con otros audios
    }
}
