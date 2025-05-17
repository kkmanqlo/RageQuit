using UnityEngine;
using DG.Tweening;



public class MovingPlatform : MonoBehaviour
{
    [Header("Movimiento Vertical")]
    public bool enableVerticalMovement = false;    // ¿Mover verticalmente?
    public float verticalDistance = 2f;            // Distancia vertical a mover
    public float verticalDuration = 2f;            // Duración del ciclo vertical

    [Header("Movimiento Horizontal")]
    public bool enableHorizontalMovement = false;  // ¿Mover horizontalmente?
    public float horizontalDistance = 3f;          // Distancia horizontal a mover
    public float horizontalDuration = 2f;          // Duración del ciclo horizontal

    [Header("Agitación Idle")]
    public bool enableIdleFloat = false;            // ¿Hacer flotación suave en idle?
    public float floatAmplitude = 0.2f;             // Amplitud del movimiento de flotación
    public float floatDuration = 1f;                 // Duración del ciclo de flotación

    private Vector3 initialPosition;                // Posición inicial de la plataforma

    private Transform playerOnPlatform;             // Referencia al jugador que está sobre la plataforma
    private Vector3 lastPlatformPosition;           // Última posición de la plataforma (para calcular movimiento)

    void Start()
    {
        lastPlatformPosition = transform.position;  // Guardar posición inicial para seguimiento
        initialPosition = transform.position;

        // Iniciar movimiento vertical con tween si está habilitado
        if (enableVerticalMovement)
        {
            transform.DOMoveY(initialPosition.y + verticalDistance, verticalDuration)
                .SetLoops(-1, LoopType.Yoyo)          // Movimiento de ida y vuelta infinito
                .SetEase(Ease.InOutSine);            // Suavizado del movimiento
        }

        // Iniciar movimiento horizontal con tween si está habilitado
        if (enableHorizontalMovement)
        {
            transform.DOMoveX(initialPosition.x + horizontalDistance, horizontalDuration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }

        // Iniciar flotación suave (idle) si está habilitada
        if (enableIdleFloat)
        {
            transform.DOMoveY(initialPosition.y + floatAmplitude, floatDuration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }
    }

    void Update()
    {
        // Si hay un jugador sobre la plataforma, moverlo junto con la plataforma para que no se quede atrás
        if (playerOnPlatform != null)
        {
            Vector3 platformMovement = transform.position - lastPlatformPosition;
            playerOnPlatform.position += platformMovement;
        }

        // Actualizar la última posición para la siguiente frame
        lastPlatformPosition = transform.position;
    }

    // Detecta cuando un objeto colisiona con la plataforma
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Si el objeto que colisiona es el jugador (tiene CharacterMovement)
        if (collision.gameObject.GetComponent<CharacterMovement>())
        {
            playerOnPlatform = collision.transform;  // Guardar referencia para moverlo con la plataforma
        }
    }

    // Detecta cuando un objeto deja de colisionar con la plataforma
    private void OnCollisionExit2D(Collision2D collision)
    {
        // Si el objeto que sale es el jugador, quitar la referencia para dejar de moverlo
        if (collision.gameObject.GetComponent<CharacterMovement>())
        {
            playerOnPlatform = null;
        }
    }
}
