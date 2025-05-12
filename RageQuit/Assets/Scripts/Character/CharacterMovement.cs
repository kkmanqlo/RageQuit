using System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem.LowLevel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class CharacterMovement : MonoBehaviour
{
    //Metodos públicos para que se puedan modificar desde el editor// LayerMask para el suelo
    public float Speed;
    public float JumpForce;

    // Coyote Time
    private float coyoteTime = 0.11f; // Tiempo máximo para saltar después de dejar el suelo
    private float coyoteTimer;

    private bool hasJumpBuffered = false;

    //Metodos privados para que no se puedan modificar desde el editor
    private Rigidbody2D Rigidbody2D;
    private Queue<KeyCode> inputBuffer;
    private Animator Animator;
    private float Horizontal;
    private bool Grounded;
    private SpriteRenderer spriteRenderer;
    private Color originalColor; // Color original del sprite

    public RaycastHit2D hit;

    

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

        hit = Physics2D.Raycast(transform.position, Vector3.down, 0.15f);
        Debug.DrawRay(transform.position, Vector3.down * 0.15f, Color.red);
        if (hit)
        {
            Grounded = true;
            coyoteTimer = coyoteTime;
        }
        else
        {
            Grounded = false;
            coyoteTimer -= Time.deltaTime;

        }


        if (Input.GetKeyDown(KeyCode.W) && inputBuffer.Count == 0  && !hasJumpBuffered)
        {
            inputBuffer.Enqueue(KeyCode.W);
            hasJumpBuffered = true;
            Invoke("quitarAccion", 0.3f); // Llamar a quitarAccion después de 0.5 segundos
        }

        if (coyoteTimer > 0) // Solo si está realmente en el suelo
        {
            if (inputBuffer.Count > 0 && inputBuffer.Peek() == KeyCode.W)
            {
                coyoteTimer = 0;
                Jump();
                inputBuffer.Dequeue();
            }
        }

    }

    //Metodo para saltar
    private void Jump()
    {
        Rigidbody2D.linearVelocity = new Vector2(Rigidbody2D.linearVelocity.x, 0);
        Rigidbody2D.AddForce(Vector2.up * JumpForce);
        Animator.SetTrigger("jump");
    }

    void quitarAccion()
    {
        if (inputBuffer.Count > 0)
            inputBuffer.Dequeue();
        hasJumpBuffered = false;
    }


    //El metodo FixedUpdate se llama en cada frame de fisica

    private void FixedUpdate()
    {
        Rigidbody2D.linearVelocity = new Vector2(Horizontal, Rigidbody2D.linearVelocity.y);
    }

    public void Die()
    {
        GetComponent<Animator>().enabled = false;

        string nombreEscena = SceneManager.GetActiveScene().name;
        int idNivelActual = NivelMap.GetIdNivelPorNombre(nombreEscena);

        if (idNivelActual != 1)
        {
            LevelStatsManager.Instance.RegistrarMuerte();
        }

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
