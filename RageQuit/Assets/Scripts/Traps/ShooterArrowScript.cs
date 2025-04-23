using UnityEngine;

public class ShooterArrowScript : MonoBehaviour
{
    public GameObject ArrowPrefab; 
    public Animator animator;

     private bool alreadyShot = false;
    
    void Update()
    {
        if (alreadyShot) return; 

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.down, 2f);
        Debug.DrawRay(transform.position, Vector3.down * 2f, Color.red);

        if (hit.collider != null)
        {
            CharacterMovement character = hit.collider.GetComponent<CharacterMovement>();
            if (character != null)
            {
                animator.SetTrigger("Activate");
                Shoot();
            }

        }
    }

    private void Shoot()
    {
        Vector3 spawnPosition = transform.position + Vector3.down * 0.1f;
        alreadyShot = true; 
        Instantiate(ArrowPrefab, spawnPosition, Quaternion.identity);
    }
}
