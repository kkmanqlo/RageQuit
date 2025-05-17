using UnityEngine;

public class FireScript : MonoBehaviour
{
    // Este script se encarga de manejar el comportamiento del fuego en el juego.
    // Cuando un personaje entra en contacto con el fuego, se llama a su método Die().
    private void OnTriggerEnter2D(Collider2D collision)
    {
        CharacterMovement character = collision.GetComponent<CharacterMovement>();
        if (character != null)
        {
            character.Die();
        }
    }
}
