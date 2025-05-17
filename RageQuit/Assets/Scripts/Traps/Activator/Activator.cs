using UnityEngine;
using UnityEngine.InputSystem.Haptics;
using UnityEngine.TextCore.Text;

public class Activator : MonoBehaviour
{
    public BoxCollider2D ActivatorCollider; // El collider que el raycast del jugador debe detectar.
    public Animator animator; // Animator que se activa visualmente cuando se detecta el jugador.
    public CharacterMovement character; // Referencia al jugador para acceder a su raycast (hit).
    private bool hasActivated; // Bandera para evitar activación repetida mientras se mantiene en el trigger.

    public GameObject[] shurikens; // Objetos a activar (trampas, proyectiles, etc.)

    void Start()
    {
        // Asegura que el collider de activación esté activo al empezar.
        ActivatorCollider.enabled = true;
    }

    void Update()
    {
        // Si el jugador no ha activado aún y su raycast toca este activador...
        if (!hasActivated && character.hit.collider == ActivatorCollider)
        {
            animator.SetBool("isActive", true); // Activa la animación.

            // Activa todos los shurikens asignados.
            foreach (var shuriken in shurikens)
            {
                if (shuriken != null)
                    shuriken.SetActive(true);
            }

            hasActivated = true; // Marca como activado para evitar múltiples activaciones seguidas.
        }
        // Si el raycast del jugador ya no toca el activador...
        else if (character.hit.collider != ActivatorCollider)
        {
            animator.SetBool("isActive", false); // Desactiva la animación visual.
            hasActivated = false; // Permite que se reactive si vuelve a tocar.
        }
    }
}


