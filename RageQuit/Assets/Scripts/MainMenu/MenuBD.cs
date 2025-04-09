using UnityEngine;
using UnityEngine.UI;

public class BackgroundAnimator : MonoBehaviour
{
    public Sprite[] backgroundFrames;  // Array de imágenes para la animación
    public float frameRate = 0.1f;     // Tiempo entre cada cambio de imagen

    private Image imageComponent;       // Componente Image del Panel
    private int currentFrame = 0;       // Índice de la imagen actual
    private float timer = 0f;           // Temporizador para controlar la animación

    void Start()
    {
        imageComponent = GetComponent<Image>();  // Obtener el componente Image del Panel
        if (backgroundFrames.Length > 0)
        {
            imageComponent.sprite = backgroundFrames[0];  // Establecer la primera imagen
        }
    }

    void Update()
    {
        if (backgroundFrames.Length > 0)
        {
            timer += Time.deltaTime;
            if (timer >= frameRate)
            {
                timer = 0f;
                currentFrame = (currentFrame + 1) % backgroundFrames.Length;
                imageComponent.sprite = backgroundFrames[currentFrame];  // Cambiar la imagen
            }
        }
    }
}
