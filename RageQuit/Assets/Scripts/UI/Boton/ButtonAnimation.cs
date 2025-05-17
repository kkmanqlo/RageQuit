using System;
using UnityEngine;

public class ButtonAnimation : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>(); // Obtenemos el Animator del botón
    }

    // Método llamado al hacer click en el botón
    public void OnClick()
    {
        // Reseteamos triggers para evitar conflictos de animación
        animator.ResetTrigger("Highlighted");
        animator.ResetTrigger("Pressed");
        animator.ResetTrigger("Selected");

        // Activamos el trigger "Normal" para volver al estado base
        animator.SetTrigger("Normal");

        // Activamos el trigger "Pressed" para reproducir la animación de presión
        animator.SetTrigger("Pressed");
    }

    // Método placeholder para resetear animación, aún no implementado
    internal void ResetAnimation()
    {
        throw new NotImplementedException();
    }
}

