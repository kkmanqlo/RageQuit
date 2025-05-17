using UnityEngine;
using DG.Tweening;

public class FireBoxScript : MonoBehaviour
{
    public GameObject firePrefab;            // Prefab del fuego que se va a instanciar
    public BoxCollider2D detectionTrigger;  // Collider que detecta la presencia del jugador
    public Animator animator;                // Animator para activar/desactivar animaciones
    public float activationDelay;            // Retraso antes de activar el fuego
    public float fireDuration;               // Tiempo que dura el fuego activo antes de destruirse
    public float ReactivationTime;           // Tiempo que tarda en poder activarse otra vez
    private bool activated = false;          // Controla si la trampa está activada o no

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!activated) // Solo activar si no está ya activada
        {
            CharacterMovement character = collision.GetComponent<CharacterMovement>();
            if (character != null) // Solo activar si quien entra es el jugador
            {
                activated = true;

                // Espera el tiempo de activationDelay antes de activar animación y fuego
                DOVirtual.DelayedCall(activationDelay, () =>
                {
                    animator.SetTrigger("Activate"); // Activar animación de fuego
                    SpawnFire();                      // Instanciar fuego

                    // Tras reactivationTime desactiva la animación y permite activar de nuevo
                    DOVirtual.DelayedCall(ReactivationTime, () =>
                    {
                        animator.SetTrigger("Deactivate"); 
                        activated = false;
                    });
                });
            }
        }
    }

    private void SpawnFire()
    {
        // Posición del fuego ligeramente por encima de la caja
        Vector3 spawnPosition = transform.position + Vector3.up * 0.07f;

        // Instanciar prefab del fuego
        GameObject fireInstance = Instantiate(firePrefab, spawnPosition, Quaternion.identity);

        // Destruir fuego luego de fireDuration segundos para limpiar la escena
        Destroy(fireInstance, fireDuration);
    }
}

