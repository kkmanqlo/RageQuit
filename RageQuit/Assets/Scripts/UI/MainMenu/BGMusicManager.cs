using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMusicManager : MonoBehaviour
{
    private static BGMusicManager instance;
    private AudioSource audioSource;

    // Escenas donde debe sonar la música
    private string[] escenasConMusica = { "MenuPrincipal", "DataSaveSelectionScene" };
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject); // Evita duplicados
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Escena cargada: " + scene.name);

        if (System.Array.Exists(escenasConMusica, escena => escena == scene.name))
        {
            if (!audioSource.isPlaying)
            {
                
                audioSource.Play();
            }
        }
        else
        {
            if (audioSource.isPlaying)
            {
        
                audioSource.Stop();
            }
        }
    }
}
