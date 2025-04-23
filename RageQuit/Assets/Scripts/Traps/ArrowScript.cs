using UnityEngine;

public class ArrowScript : MonoBehaviour
{
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.down;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CharacterMovement character = collision.GetComponent<CharacterMovement>();
        if (character != null)
        {
            character.Die();
        }

        // Destruir la flecha cuando toca algo (jugador, suelo, etc.)
        Destroy(gameObject);
    }
}
