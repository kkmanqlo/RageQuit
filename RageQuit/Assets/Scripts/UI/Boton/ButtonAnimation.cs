using System;
using UnityEngine;

public class ButtonAnimation : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void OnClick()
    {
        animator.ResetTrigger("Highlighted");
        animator.ResetTrigger("Pressed");
        animator.ResetTrigger("Selected");

        animator.SetTrigger("Normal");     // Vuelve a Normal (esto activa la transición)
        animator.SetTrigger("Pressed");    // Luego lanza la animación que quieres
    }

    internal void ResetAnimation()
    {
        throw new NotImplementedException();
    }
}
