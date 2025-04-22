using UnityEngine;
using UnityEngine.UIElements;

public class Spears : MonoBehaviour
{
    public Collider2D damageColliderExtendido;

    public Collider2D damageColliderRetraido;

    public void EnableDamageExtendido()
    {
        damageColliderExtendido.enabled = true;
    }

    public void DisableDamageExtendido()
    {
        damageColliderExtendido.enabled = false;
    }

    public void EnableDamageRetraido()
    {
        damageColliderRetraido.enabled = true;
    }

    public void DisableDamageRetraido()
    {
        damageColliderRetraido.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Comprobamos si el objeto que entró es el personaje
        CharacterMovement character = collision.GetComponent<CharacterMovement>();
        if (character == null) return;

        // Verificamos si fue tocado por uno de los dos colliders activos
        if (damageColliderExtendido.enabled && collision.IsTouching(damageColliderExtendido))
        {
            character.Die();
        }
        else if (damageColliderRetraido.enabled && collision.IsTouching(damageColliderRetraido))
        {
            character.Die();
        }
    }

}
