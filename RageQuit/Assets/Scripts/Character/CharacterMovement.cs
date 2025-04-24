using System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem.LowLevel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class CharacterMovement : MonoBehaviour
{
    //Metodos públicos para que se puedan modificar desde el editor
    public float Speed;
    public float JumpForce;

    //Metodos privados para que no se puedan modificar desde el editor
    private Rigidbody2D Rigidbody2D;
    private Queue<KeyCode> inputBuffer;
    private Animator Animator;
    private float Horizontal;
    private bool Grounded;
    private SpriteRenderer spriteRenderer;
    private Color originalColor; // Color original del sprite

    Vector2 startPosition;

    // Se llama al inicio del juego
    void Start()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        startPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color; // Guardar el color original
        inputBuffer = new Queue<KeyCode>();

    }

    // Se llama una vez por frame para actualizar la lógica del juego
    void Update()
    {
        Horizontal = Input.GetAxisRaw("Horizontal");

        if (Horizontal < 0.0f) transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        else if (Horizontal > 0.0f) transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        Animator.SetBool("grounded", Grounded);
        Animator.SetBool("running", Grounded && Horizontal != 0.0f);
        Animator.SetBool("falling", !Grounded && Rigidbody2D.linearVelocity.y < 0.0f);
        Animator.SetFloat("yvelocity", Rigidbody2D.linearVelocity.y);
        Animator.SetFloat("xvelocity", Rigidbody2D.linearVelocity.x);


        Debug.DrawRay(transform.position, Vector3.down * 0.18f, Color.red);
        if (Physics2D.Raycast(transform.position, Vector3.down, 0.18f))
        {
            Grounded = true;
        }
        else Grounded = false;


        if (Input.GetKeyDown(KeyCode.W))
        {
            inputBuffer.Enqueue(KeyCode.W);
            Invoke("quitarAccion", 0.1f); // Llamar a quitarAccion después de 0.5 segundos
        }

        if (Grounded)
        {
            if (inputBuffer.Count > 0)
            {
                if (inputBuffer.Peek() == KeyCode.W)
                {
                    Jump();
                    inputBuffer.Dequeue();
                }
            }
        }

        Debug.Log("Grounded: " + Grounded + " | yVelocity: " + Rigidbody2D.linearVelocity.y + " | CurrentAnim: " + Animator.GetCurrentAnimatorStateInfo(0).IsName("Jump"));
    }

    //Metodo para saltar
    private void Jump()
    {
        Rigidbody2D.AddForce(Vector2.up * JumpForce);
        Animator.SetTrigger("jump");
    }

    void quitarAccion()
    {
        if (inputBuffer.Count > 0)
            inputBuffer.Dequeue();
    }


    //El metodo FixedUpdate se llama en cada frame de fisica

    private void FixedUpdate()
    {
        Rigidbody2D.linearVelocity = new Vector2(Horizontal, Rigidbody2D.linearVelocity.y);
    }

    public void Die()
{
    spriteRenderer.DOColor(Color.red, 0.05f).OnComplete(() =>
    {
        Time.timeScale = 0f;

        DOVirtual.DelayedCall(0.2f, () =>
        {
            Time.timeScale = 1f;

            // Cancelar todos los tweens activos para evitar errores de objetos destruidos
            DOTween.KillAll();

            // Recargar escena
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }).SetUpdate(true);
    });
}


}
