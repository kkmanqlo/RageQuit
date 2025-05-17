using UnityEngine;
using DG.Tweening;

public class SpikeRaiseScript : MonoBehaviour
{
    public BoxCollider2D detectionTrigger; // Trigger que detecta la presencia del jugador
    public BoxCollider2D spikeCollider;    // Collider que causa daño, activado con animación
    public Animator animator;               // Controla la animación de los pinchos
    public float activationDelay;           // Retraso antes de activar la animación

    private bool activated = false;         // Flag para evitar activaciones múltiples

    void Start()
    {
        spikeCollider.enabled = false; // Desactivamos el collider de daño al inicio
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Solo activar si el collider que entró está tocando el trigger de detección y no está activado
        if (collision.IsTouching(detectionTrigger) && !activated)
        {
            CharacterMovement character = collision.GetComponent<CharacterMovement>();
            if (character != null) // Verificamos que sea el jugador
            {
                activated = true; // Marcamos como activado para no repetir

                // Ejecutamos la activación con delay usando DOTween
                DOVirtual.DelayedCall(activationDelay, () =>
                {
                    animator.SetTrigger("Activate"); // Lanzamos la animación
                });
            }
        }
    }

    // Método llamado por la animación para activar el collider de daño
    public void EnableDamageExtendido()
    {
        spikeCollider.enabled = true;
    }

    // Método llamado por la animación para desactivar el collider de daño
    public void DisableDamageExtendido()
    {
        spikeCollider.enabled = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Verificamos si el jugador colisiona y el daño está activado
        CharacterMovement character = collision.collider.GetComponent<CharacterMovement>();
        if (character != null && spikeCollider.enabled && collision.collider.IsTouching(spikeCollider))
        {
            character.Die(); // Matar al jugador si está tocando los pinchos activos
        }
    }
}

