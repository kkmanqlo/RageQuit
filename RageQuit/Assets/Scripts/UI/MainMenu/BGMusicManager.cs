using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMusicManager : MonoBehaviour
{
    // Instancia estática para implementar el patrón Singleton y evitar duplicados
    private static BGMusicManager instance;

    // Referencia al componente AudioSource que reproduce la música
    private AudioSource audioSource;

    // Array con los nombres de las escenas donde debe sonar la música de fondo
    private string[] escenasConMusica = { "MenuPrincipal", "DataSaveSelectionScene", "LevelSelectionScene" };

    void Awake()
    {
        // Si no hay instancia previa, se asigna esta y se evita que se destruya al cambiar de escena
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);               // Persistir entre escenas
            audioSource = GetComponent<AudioSource>();   // Obtener el AudioSource adjunto
            SceneManager.sceneLoaded += OnSceneLoaded;   // Suscribirse al evento de cambio de escena
        }
        else
        {
            // Si ya existe una instancia, destruir esta para evitar duplicados
            Destroy(gameObject);
        }
    }

    // Método llamado automáticamente cada vez que se carga una escena
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Escena cargada: " + scene.name);

        // Verificar si la escena actual está en la lista donde debe sonar música
        if (System.Array.Exists(escenasConMusica, escena => escena == scene.name))
        {
            // Si la música no está sonando, iniciarla
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            // Si la escena no está en la lista, detener la música si está sonando
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }
}

