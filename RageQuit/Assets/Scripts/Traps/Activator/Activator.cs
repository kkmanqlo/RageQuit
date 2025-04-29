using UnityEngine;
using UnityEngine.InputSystem.Haptics;
using UnityEngine.TextCore.Text;

public class Activator : MonoBehaviour
{

    public BoxCollider2D ActivatorCollider; // El collider que detecta al jugador

    public Animator animator;

    public CharacterMovement character;
    private bool hasActivated;

    public GameObject[] shurikens;


    void Start()
    {
        // Desactivar el collider de daño al inicio
        ActivatorCollider.enabled = true;
    }

    void Update()
    {
        if (!hasActivated && character.hit.collider == ActivatorCollider)
        {
            animator.SetBool("isActive", true);
            foreach (var shuriken in shurikens)
            {
                if (shuriken != null)
                    shuriken.SetActive(true);
            }
            hasActivated = true; // Evita activarlos más veces
        }
        else if (character.hit.collider != ActivatorCollider)
        {
            animator.SetBool("isActive", false);
            hasActivated = false; // Permitirá activarlo otra vez si vuelve a entrar
        }

    }

}

