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

    private void Start()
    {
        InvokeRepeating(nameof(Shoot), 0f, fireInterval);
    }

    private void Shoot()
    {
        float[] yOffsets = new float[] { -0.2f, 0f, 0.2f }; // Ajusta para separar verticalmente los proyectiles

        for (int i = 0; i < CannonProjectiles.Length; i++)
        {
            if (CannonProjectiles[i] == null) continue;

            // Calcula la posición de disparo con el offset vertical
            Vector3 spawnPosition = transform.position + new Vector3(1f, yOffsets[i], 0.1f);
            Vector3 destination = spawnPosition + direction.normalized * distance;

            GameObject projectile = Instantiate(CannonProjectiles[i], spawnPosition, Quaternion.identity);

            projectile.transform.DOMove(destination, duration)
                .SetEase(Ease.Linear)
                .OnComplete(() => Destroy(projectile));
        }
    }
}
