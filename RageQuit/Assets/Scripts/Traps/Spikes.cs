using UnityEngine;

public class Spikes : MonoBehaviour
{
    // Este script se encarga de manejar el comportamiento de los pinchos en el juego.
    // Cuando un personaje entra en contacto con los pinchos, se llama a su método Die().
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.GetComponent<CharacterMovement>()){
            collision.GetComponent<CharacterMovement>().Die();
        }
    }
}
 