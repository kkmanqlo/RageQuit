using UnityEngine;

public class FireScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        CharacterMovement character = collision.GetComponent<CharacterMovement>();
        if (character != null)
        {
            character.Die();
        }
    }
}
