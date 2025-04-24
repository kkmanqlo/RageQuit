using UnityEngine;

public class SpearsDesfasadas : MonoBehaviour
{
    public string animationState = "Spear";
    [Range(0f, 1f)] public float cycleOffset;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();

        // MUY IMPORTANTE: cambiar UpdateMode a 'Normal' en el Animator Inspector
        animator.updateMode = AnimatorUpdateMode.Normal;

        // Forzar a reproducir la animación desde un punto desfasado
        animator.Play(animationState, 0, cycleOffset);
    }
}
