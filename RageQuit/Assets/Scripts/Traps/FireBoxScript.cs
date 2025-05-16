using UnityEngine;
using DG.Tweening;

public class FireBoxScript : MonoBehaviour
{
    public GameObject firePrefab;
    public BoxCollider2D detectionTrigger;
    public Animator animator;
    public float activationDelay;
    public float fireDuration;
    public float ReactivationTime;
    private bool activated = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!activated)
        {
            CharacterMovement character = collision.GetComponent<CharacterMovement>();
            if (character != null)
            {
                activated = true;

                DOVirtual.DelayedCall(activationDelay, () =>
                {
                    animator.SetTrigger("Activate");
                    SpawnFire();

                    DOVirtual.DelayedCall(ReactivationTime, () =>
                    {
                        animator.SetTrigger("Deactivate");
                        activated = false;
                    });
                });
            }
        }
    }



    private void SpawnFire()
    {
        Vector3 spawnPosition = transform.position + Vector3.up * 0.07f;
        GameObject fireInstance = Instantiate(firePrefab, spawnPosition, Quaternion.identity);
        Destroy(fireInstance, fireDuration);
    }
}
