using UnityEngine;
using UnityEngine.UIElements;

public class Spears : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Intentamos obtener el componente CharacterMovement del objeto que colisiona
        CharacterMovement character = collision.GetComponent<CharacterMovement>();
        
        // Si no tiene CharacterMovement, no hacemos nada y salimos
        if (character == null) return;

        // Verificamos si el collider está tocando el collider de esta lanza
        if (collision.IsTouching(GetComponent<Collider2D>()))
        {
            // Desactivamos el Animator para detener la animación de la lanza
            GetComponent<Animator>().enabled = false;
            
            // Llamamos al método Die() del personaje para que muera
            character.Die();
        }
    }
}

