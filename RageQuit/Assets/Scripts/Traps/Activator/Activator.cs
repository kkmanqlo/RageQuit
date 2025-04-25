using UnityEngine;
using UnityEngine.InputSystem.Haptics;
using UnityEngine.TextCore.Text;

public class Activator : MonoBehaviour
{

    public BoxCollider2D ActivatorCollider; // El collider que detecta al jugador

    public Animator animator;

    public CharacterMovement character;


    void Start()
    {
        // Desactivar el collider de daño al inicio
        ActivatorCollider.enabled = true;
    }

    void Update()
    {
        if (character.hit.collider == ActivatorCollider)
        {
            animator.SetBool("isActive", true);
        }
        else animator.SetBool("isActive", false);

    }

}

