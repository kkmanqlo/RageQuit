using UnityEngine;
using DG.Tweening;

public class Cannon_Horizontal : MonoBehaviour
{
    [Header("Movimiento del Proyectil")]
    public Vector3 direction = Vector3.right; // Dirección en la que se disparan los proyectiles
    public float distance;                    // Distancia que recorrerá cada proyectil
    public float duration;                    // Tiempo que tardará en recorrer esa distancia

    public GameObject[] CannonProjectiles;    // Array de proyectiles distintos que puede disparar
    public float fireInterval;                // Tiempo entre cada ráfaga/disparo

    [Header("Movimiento Vertical")]
    public bool enableVerticalMovement = false; // ¿Se mueve el cañón verticalmente?
    public float verticalDistance = 2f;         // Cuánto se moverá en Y
    public float verticalDuration = 2f;         // Cuánto tarda en subir/bajar

    [Header("Movimiento Horizontal")]
    public bool enableHorizontalMovement = false; // ¿Se mueve el cañón horizontalmente?
    public float horizontalDistance = 3f;         // Cuánto se moverá en X
    public float horizontalDuration = 2f;         // Cuánto tarda en ir y volver

    [Header("Agitación Idle")]
    public bool enableIdleFloat = false;      // ¿Agitación constante en idle?
    public float floatAmplitude = 0.2f;       // Qué tan alto/bajo "flota"
    public float floatDuration = 1f;          // Tiempo entre subida y bajada

    private Vector3 initialPosition;          // Posición original del cañón

    [Header("Separación entre proyectiles")]
    public float[] offsets = new float[] { 0f, 0f, 0f }; // Desfase horizontal individual para cada proyectil
    public float Y = 0f;                                 // Desfase vertical fijo para todos

    private void Start()
    {
        // Comienza a disparar a intervalos regulares
        InvokeRepeating(nameof(Shoot), 0f, fireInterval);

        // Guarda la posición inicial del cañón
        initialPosition = transform.position;

        // Movimiento vertical (tipo plataforma que sube y baja)
        if (enableVerticalMovement)
        {
            transform.DOMoveY(initialPosition.y + verticalDistance, verticalDuration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine); // Movimiento suave
        }

        // Movimiento horizontal (tipo plataforma que va y viene)
        if (enableHorizontalMovement)
        {
            transform.DOMoveX(initialPosition.x + horizontalDistance, horizontalDuration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }

        // Efecto visual de flotación constante (idle float)
        if (enableIdleFloat)
        {
            transform.DOMoveY(initialPosition.y + floatAmplitude, floatDuration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }
    }

    private void Shoot()
    {
        // Recorre todos los proyectiles definidos en el array
        for (int i = 0; i < CannonProjectiles.Length; i++)
        {
            if (CannonProjectiles[i] == null) continue;

            // Calcula la posición de aparición del proyectil (con offset individual)
            Vector3 spawnPosition = transform.position + new Vector3(offsets[i], Y, 0f);
            Vector3 destination = spawnPosition + direction.normalized * distance;

            // Instancia el proyectil y lo mueve hacia la dirección definida
            GameObject projectile = Instantiate(CannonProjectiles[i], spawnPosition, Quaternion.identity);

            projectile.transform.DOMove(destination, duration)
                .SetEase(Ease.Linear)              // Movimiento constante
                .OnComplete(() => Destroy(projectile)); // Destruye al llegar
        }
    }
}

