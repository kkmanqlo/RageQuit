using UnityEngine;
using DG.Tweening;
using System.Collections;

public class Cannon : MonoBehaviour
{
    [Header("Movimiento del Proyectil")]
    public Vector3 direction = Vector3.right; // Dirección por defecto
    public float distance;
    public float duration;

    public GameObject[] CannonProjectiles; // Array de proyectiles (por ejemplo, 3 diferentes)
    public float fireInterval; // Intervalo entre disparos

    [Header("Movimiento Vertical")]
    public bool enableVerticalMovement = false;
    public float verticalDistance = 2f;
    public float verticalDuration = 2f;

    [Header("Movimiento Horizontal")]
    public bool enableHorizontalMovement = false;
    public float horizontalDistance = 3f;
    public float horizontalDuration = 2f;

    [Header("Agitación Idle")]
    public bool enableIdleFloat = false;
    public float floatAmplitude = 0.2f;
    public float floatDuration = 1f;

    private Vector3 initialPosition;

    [Header("Separación entre proyectiles")]
    public float[] offsets = new float[] { 0f, 0f, 0f };
    public float X = 0f;


    private void Start()
    {
        InvokeRepeating(nameof(Shoot), 0f, fireInterval);

        initialPosition = transform.position;

        if (enableVerticalMovement)
        {
            transform.DOMoveY(initialPosition.y + verticalDistance, verticalDuration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }

        if (enableHorizontalMovement)
        {
            transform.DOMoveX(initialPosition.x + horizontalDistance, horizontalDuration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }

        if (enableIdleFloat)
        {
            transform.DOMoveY(initialPosition.y + floatAmplitude, floatDuration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }
    }


    private void Shoot()
    { 

        for (int i = 0; i < CannonProjectiles.Length; i++)
        {
            if (CannonProjectiles[i] == null) continue;

            // Calcula la posición de disparo con el offset vertical
            Vector3 spawnPosition = transform.position + new Vector3(X, offsets[i], 0f);
            Vector3 destination = spawnPosition + direction.normalized * distance;

            GameObject projectile = Instantiate(CannonProjectiles[i], spawnPosition, Quaternion.identity);

            projectile.transform.DOMove(destination, duration)
                .SetEase(Ease.Linear)
                .OnComplete(() => Destroy(projectile));
        }
    }
}
