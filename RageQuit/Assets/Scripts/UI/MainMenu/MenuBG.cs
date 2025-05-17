using UnityEngine;
using UnityEngine.UI;

public class BackgroundAnimator : MonoBehaviour
{
    public Sprite[] backgroundFrames;  // Array con las imágenes que formarán la animación de fondo
    public float frameRate = 0.1f;     // Tiempo (en segundos) que pasa entre cada cambio de imagen

    private Image imageComponent;       // Referencia al componente Image donde se mostrará la animación
    private int currentFrame = 0;       // Índice del frame (imagen) actual que se está mostrando
    private float timer = 0f;           // Temporizador para controlar cuándo cambiar de frame

    void Start()
    {
        // Obtener el componente Image del GameObject al que está asignado este script
        imageComponent = GetComponent<Image>();

        // Si el array de frames tiene al menos una imagen, establecer la primera
        if (backgroundFrames.Length > 0)
        {
            imageComponent.sprite = backgroundFrames[0];
        }
    }

    void Update()
    {
        // Si hay frames para animar
        if (backgroundFrames.Length > 0)
        {
            // Incrementar el temporizador con el tiempo transcurrido desde el último frame
            timer += Time.deltaTime;

            // Cuando el temporizador supera el tiempo definido para cambiar de frame
            if (timer >= frameRate)
            {
                timer = 0f; // Reiniciar el temporizador

                // Avanzar al siguiente frame, volviendo al inicio si se pasa del último
                currentFrame = (currentFrame + 1) % backgroundFrames.Length;

                // Actualizar la imagen mostrada con el nuevo frame
                imageComponent.sprite = backgroundFrames[currentFrame];
            }
        }
    }
}

