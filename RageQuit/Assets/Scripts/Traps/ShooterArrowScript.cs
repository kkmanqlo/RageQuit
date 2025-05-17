using UnityEngine;

public class ShooterArrowScript : MonoBehaviour
{
    public GameObject ArrowPrefab;    // Prefab de la flecha que se disparará
    public Animator animator;         // Animator para controlar la animación de disparo

    private bool alreadyShot = false; // Flag para evitar disparos múltiples seguidos
    public float raycastLength = 2f;  // Longitud del raycast hacia abajo para detectar al jugador

    void Update()
    {
        if (alreadyShot) return;  // Si ya disparó, no hacer nada

        // Lanzar un raycast hacia abajo desde la posición actual
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.down, raycastLength);
        Debug.DrawRay(transform.position, Vector3.down * raycastLength, Color.red); // Línea roja para debug en escena

        if (hit.collider != null) // Si el raycast golpea algo
        {
            // Intentar obtener el componente CharacterMovement para verificar si es el jugador
            CharacterMovement character = hit.collider.GetComponent<CharacterMovement>();
            if (character != null) // Si es el jugador, activar animación y disparar
            {
                animator.SetTrigger("Activate");
                Shoot();
            }
        }
    }

    private void Shoot()
    {
        // Posición ligeramente por debajo para instanciar la flecha
        Vector3 spawnPosition = transform.position + Vector3.down * 0.1f;

        alreadyShot = true; // Marcar que ya disparó para no disparar otra vez sin reset

        // Instanciar la flecha en la posición calculada, sin rotación
        Instantiate(ArrowPrefab, spawnPosition, Quaternion.identity);
    }
}

