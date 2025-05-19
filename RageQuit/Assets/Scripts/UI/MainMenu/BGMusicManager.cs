using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMusicManager : MonoBehaviour
{
    private static BGMusicManager instance;
    private AudioSource audioSource;

    [Header("Clips de música")]
    public AudioClip menuMusic;
    public AudioClip levelMusic;

    // Escenas del menú
    private string[] escenasMenu = { "MenuPrincipal", "DataSaveSelectionScene", "LevelSelectionScene" };

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();
            audioSource.loop = true;

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string sceneName = scene.name;
        Debug.Log("Escena cargada: " + sceneName);

        AudioClip targetClip = System.Array.Exists(escenasMenu, e => e == sceneName) ? menuMusic : levelMusic;

        if (audioSource.clip != targetClip)
        {
            audioSource.clip = targetClip;
            audioSource.Play();
        }
    }
}
