using UnityEngine;
using UnityEngine.UIElements;

public class Spears : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CharacterMovement character = collision.GetComponent<CharacterMovement>();
        if (character == null) return;

        // Verificamos si fue tocado por el shuriken
        if (collision.IsTouching(GetComponent<Collider2D>()))
        {
            GetComponent<Animator>().enabled = false;
            character.Die();
        }
    }

}
