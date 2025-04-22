using UnityEngine;
using DG.Tweening;

public class SpikeRaiseScript : MonoBehaviour
{
    public BoxCollider2D detectionTrigger; // El trigger que detecta al jugador
    public BoxCollider2D spikeCollider;    // El que hace daño (activado con animación)
    public Animator animator;
    public float activationDelay = 1f;

    private bool activated = false;

    void Start()
    {
        spikeCollider.enabled = false; // El hitbox de daño empieza desactivado
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Aseguramos que solo se active si el trigger es el de detección
        if (collision.IsTouching(detectionTrigger) && !activated)
        {
            CharacterMovement character = collision.GetComponent<CharacterMovement>();
            if (character != null)
            {
                activated = true;

                // Activamos animación con delay usando DOTween
                DOVirtual.DelayedCall(activationDelay, () =>
                {
                    animator.SetTrigger("Activate");
                });
            }
        }
    }

    public void EnableDamageExtendido()
    {
        spikeCollider.enabled = true;
    }

    public void DisableDamageExtendido()
    {
        spikeCollider.enabled = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CharacterMovement character = collision.collider.GetComponent<CharacterMovement>();
        if (character != null && spikeCollider.enabled && collision.collider.IsTouching(spikeCollider))
        {
            character.Die();
        }
    }
}
