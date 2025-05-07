using UnityEngine;
using DG.Tweening;

public class JumperScript : MonoBehaviour
{
    public Animator animator;
    public float jumpForce = 10f;
    public float resetTime = 0.2f;

    private bool activated = false;


    private void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!activated)
        {
            CharacterMovement character = collision.GetComponent<CharacterMovement>();
            if (character != null)
            {
                Rigidbody2D rb = character.GetComponent<Rigidbody2D>();
                Animator anim = character.GetComponent<Animator>();
                

                if (rb != null)
                {
                    // Reiniciar velocidad vertical para mantener salto constante
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
                    rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

                    // Activar animación y flag
                    activated = true;
                    animator.SetTrigger("Activate");
                    anim.SetTrigger("jump");
                    
                    // Llamar a la desactivación con retardo
                    Invoke(nameof(ResetActivation), resetTime);
                }
            }
        }
        
    }

        private void ResetActivation()
    {
        activated = false;
        animator.SetTrigger("Deactivate");
    }
}
