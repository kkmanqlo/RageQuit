using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevelDoorScript : MonoBehaviour
{
    [Tooltip("¿Usar siguiente escena en el build index automáticamente?")]
    public bool autoLoadNextScene = true;

    [Tooltip("Si no es automático, nombra aquí la escena a cargar")]
    public string sceneToLoad;

    private bool activated = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (activated) return;

        CharacterMovement character = collision.GetComponent<CharacterMovement>();
        if (character != null)
        {
            activated = true;
            Invoke(nameof(LoadScene), 0.5f);
        }
    }

    private void LoadScene()
    {
        DOTween.KillAll();

        if (autoLoadNextScene)
        {
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(nextSceneIndex);
            }
            else
            {
                Debug.LogWarning("No hay más escenas en el build index.");
            }
        }
        else
        {
            if (!string.IsNullOrEmpty(sceneToLoad))
            {
                SceneManager.LoadScene(sceneToLoad);
            }
            else
            {
                Debug.LogError("No se ha asignado el nombre de la escena a cargar.");
            }
        }
    }

}
