using UnityEngine;
using DG.Tweening;

public class ShurikenProjectile : MonoBehaviour
{
    [Header("Movimiento del shuriken")]
    public Vector3 direction = new Vector3();  // Dirección en la que se moverá el shuriken
    public float distance;                     // Distancia que recorrerá el shuriken
    public float duration;                     // Duración del movimiento hasta el destino

    void OnEnable()
    {
        // Calcula el punto destino sumando la dirección normalizada multiplicada por la distancia
        Vector3 destination = transform.position + direction.normalized * distance;

        // Inicia el movimiento del shuriken hacia el destino con DOTween, con movimiento lineal
        transform.DOMove(destination, duration)
            .SetEase(Ease.Linear)               // Movimiento con velocidad constante
            .OnComplete(() =>
            {
                // Al finalizar el movimiento, destruye este objeto shuriken
                Destroy(gameObject);
            });
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Obtiene el componente CharacterMovement del objeto colisionado, si existe
        CharacterMovement character = collision.GetComponent<CharacterMovement>();

        // Si no hay CharacterMovement, no hace nada
        if (character == null) return;

        // Comprueba que el collider está tocando el collider de este objeto
        if (collision.IsTouching(GetComponent<Collider2D>()))
        {
            // Detiene todas las animaciones/tweens activos (DOTween)
            DOTween.KillAll();

            // Llama al método Die() del personaje para que muera
            character.Die();
        }
    }
}

