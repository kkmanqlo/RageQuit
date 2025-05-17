using UnityEngine;
using DG.Tweening;

public class CannonProjectiles : MonoBehaviour
{
    // Método que se llama cuando otro collider entra en contacto con el collider de este proyectil
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Intentamos obtener el componente CharacterMovement del objeto que colisionó
        CharacterMovement character = collision.GetComponent<CharacterMovement>();

        // Si no es el jugador, no hacemos nada
        if (character == null) return;

        // Verificamos que efectivamente esté tocando este collider (por seguridad)
        if (collision.IsTouching(GetComponent<Collider2D>()))
        {
            // Detenemos todas las animaciones y tweens activos (probablemente para evitar conflictos al morir)
            DOTween.KillAll();

            // Llamamos al método de muerte del personaje
            character.Die();
        }
    }
}

