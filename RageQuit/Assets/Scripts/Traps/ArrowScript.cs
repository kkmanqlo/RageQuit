using UnityEngine;

public class ArrowScript : MonoBehaviour
{
    private Rigidbody2D rb;

    void Start()
    {
        // Obtener el componente Rigidbody2D para controlar la física
        rb = GetComponent<Rigidbody2D>();
        
        // Asignar una velocidad lineal hacia abajo para que la flecha caiga
        rb.linearVelocity = Vector2.down;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Intentar obtener el componente CharacterMovement del objeto que colisiona
        CharacterMovement character = collision.GetComponent<CharacterMovement>();
        
        // Si colisiona con el jugador, llamar a su función de muerte
        if (character != null)
        {
            character.Die();
        }

        // Destruir la flecha al tocar cualquier objeto para evitar que siga existiendo
        Destroy(gameObject);
    }
}

