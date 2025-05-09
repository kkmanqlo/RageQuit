using UnityEngine;
using DG.Tweening;

public class CannonProjectiles : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter2D(Collider2D collision)
    {
        CharacterMovement character = collision.GetComponent<CharacterMovement>();
        if (character == null) return;

        // Verificamos si fue tocado por la bala de cañon
        if (collision.IsTouching(GetComponent<Collider2D>()))
        {
            DOTween.KillAll();
            character.Die();
            
        }
    }
}
