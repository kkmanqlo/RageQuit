using UnityEngine;
using DG.Tweening;

public class ShurikenProjectile : MonoBehaviour
{
    [Header("Movimiento del shuriken")]
    public Vector3 direction = new Vector3(); // Dirección por defecto: diagonal abajo-derecha
    public float distance;
    public float duration;

    void OnEnable()
    {
        // Calcula la posición final basándose en la dirección normalizada
        Vector3 destination = transform.position + direction.normalized * distance;

        // Movimiento usando DOTween
        transform.DOMove(destination, duration)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                Destroy(gameObject);
            });
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Comprobamos si el objeto que entró es el personaje
        CharacterMovement character = collision.GetComponent<CharacterMovement>();
        if (character == null) return;

        // Verificamos si fue tocado por el shuriken
        if (collision.IsTouching(GetComponent<Collider2D>()))
        {
            character.Die();
        }
    }
}