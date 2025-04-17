using System;
using UnityEngine;
using DG.Tweening; 

public class CharacterMovement : MonoBehaviour
{
    //Metodos públicos para que se puedan modificar desde el editor
    public float Speed;
    public float JumpForce;

    //Metodos privados para que no se puedan modificar desde el editor
    private Rigidbody2D Rigidbody2D;
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

    }

    // Se llama una vez por frame para actualizar la lógica del juego
    void Update()
    {
        Horizontal = Input.GetAxisRaw("Horizontal");

        if (Horizontal < 0.0f) transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        else if (Horizontal > 0.0f) transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        Animator.SetBool("grounded", Grounded);
        Animator.SetBool("running", Grounded && Horizontal != 0.0f);
        Animator.SetFloat("yvelocity", Rigidbody2D.linearVelocity.y);
        

        Debug.DrawRay(transform.position, Vector3.down * 0.16f, Color.red);
        if (Physics2D.Raycast(transform.position, Vector3.down, 0.16f))
        {
            Grounded = true;
        }
        else Grounded = false;
        

        if (Input.GetKeyDown(KeyCode.W) && Grounded)
        {
            Jump();
        }
    }

    //Metodo para saltar
    private void Jump()
    {
        Rigidbody2D.AddForce(Vector2.up * JumpForce);
        Animator.SetTrigger("jump");
    }

    //El metodo FixedUpdate se llama en cada frame de fisica

    private void FixedUpdate()
    {
        Rigidbody2D.linearVelocity = new Vector2(Horizontal, Rigidbody2D.linearVelocity.y);
    }

    public void Die()
    {
        // Efecto rojo
        spriteRenderer.DOColor(Color.red, 0.05f).OnComplete(() =>
        {
            DOVirtual.DelayedCall(0.02f, () =>
            {
                spriteRenderer.DOColor(originalColor, 0.05f);
                // Reiniciar posición después del parpadeo
                transform.position = startPosition;
            });
        });
    }
}
