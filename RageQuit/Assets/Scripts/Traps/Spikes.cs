using UnityEngine;

public class Spikes : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.GetComponent<CharacterMovement>()){
            collision.GetComponent<CharacterMovement>().Die();
        }
    }
}
 