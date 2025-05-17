using UnityEngine;
using DG.Tweening;

public class JumperScript : MonoBehaviour
{
    public Animator animator;        // Animator del jumper para activar animaciones
    public float jumpForce = 10f;    // Fuerza del salto que se aplica al jugador
    public float resetTime = 0.2f;   // Tiempo que tarda en poder activarse nuevamente

    private bool activated = false;  // Flag para evitar activaciones múltiples consecutivas

    private void Update()
    {
        // Vacío, no se usa en este script
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!activated) // Solo si no está activado actualmente
        {
            CharacterMovement character = collision.GetComponent<CharacterMovement>();
            if (character != null) // Si el objeto que entra es el jugador
            {
                Rigidbody2D rb = character.GetComponent<Rigidbody2D>();
                Animator anim = character.GetComponent<Animator>();

                if (rb != null)
                {
                    // Resetear velocidad vertical para salto consistente
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

                    // Aplicar fuerza vertical de impulso para saltar
                    rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

                    // Marcar como activado para evitar múltiples activaciones rápidas
                    activated = true;

                    // Activar animación del jumper y la del jugador (salto)
                    animator.SetTrigger("Activate");
                    anim.SetTrigger("jump");

                    // Programar reseteo de activación para permitir que salte otra vez
                    Invoke(nameof(ResetActivation), resetTime);
                }
            }
        }
    }

    private void ResetActivation()
    {
        activated = false;               // Permitir nuevas activaciones
        animator.SetTrigger("Deactivate"); // Desactivar animación del jumper
    }
}

