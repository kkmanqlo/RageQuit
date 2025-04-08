using UnityEngine;

public class Spikes : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.GetComponent<CharacterMovement>()){
            collision.collider.GetComponent<CharacterMovement>().Die();
        }
    }
}
 