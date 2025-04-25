using UnityEngine;
using DG.Tweening;

public class JumperScript : MonoBehaviour
{
    public CircleCollider2D detectionTrigger;
    public Animator animator;
    public float jumpForce;

    private bool activated = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.IsTouching(detectionTrigger) && !activated)
        {
            CharacterMovement character = collision.GetComponent<CharacterMovement>();
            if (character != null)
            {
                activated = true;

                animator.SetTrigger("Activate");
                character.GetComponent<Rigidbody2D>().AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

                animator.SetTrigger("Deactivate");
                activated = false;


            }
        }
    }



}
