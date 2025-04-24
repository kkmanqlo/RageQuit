using UnityEngine;

public class SpearsDesfasadas : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public string nombreAnimacion = "Spear";
    [Range(0f, 1f)] public float inicioEnCiclo = 0f; // 0 = inicio, 0.5 = mitad, 0.9 = casi al final

    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();

        // Forzamos a reproducir la animación en un punto determinado del ciclo
        anim.Play(nombreAnimacion, 0, inicioEnCiclo);
    }
}
