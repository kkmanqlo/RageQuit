using System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem.LowLevel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class CharacterMovement : MonoBehaviour
{
    public float Speed;
    public float JumpForce;

    private float coyoteTime = 0.11f;
    private float coyoteTimer;

    private bool hasJumpBuffered = false;

    private Rigidbody2D Rigidbody2D;
    private Queue<KeyCode> inputBuffer;
    private Animator Animator;
    private float Horizontal;
    private bool Grounded;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    public RaycastHit2D hit;

    public LayerMask groundLayer;  // Asigna en el inspector la capa suelo

    // Ajusta esta altura para que el raycast salga desde cerca de los pies
    private float offsetY = 0f;

    void Start()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
        Rigidbody2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        Animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        inputBuffer = new Queue<KeyCode>();
    }

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

        // Posición desde donde lanzamos el raycast (cerca de los pies)
        Vector2 origenRaycast = (Vector2)transform.position + Vector2.down * offsetY;
        float distanciaRaycast = 0.15f;

        hit = Physics2D.Raycast(origenRaycast, Vector2.down, distanciaRaycast, groundLayer);
        Debug.DrawRay(origenRaycast, Vector2.down * distanciaRaycast, hit.collider != null ? Color.green : Color.red);

        if (hit.collider != null)
        {
            Grounded = true;
            coyoteTimer = coyoteTime;
        }
        else
        {
            Grounded = false;
            coyoteTimer -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.W) && inputBuffer.Count == 0 && !hasJumpBuffered)
        {
            inputBuffer.Enqueue(KeyCode.W);
            hasJumpBuffered = true;
            Invoke("quitarAccion", 0.3f);
        }

        if (coyoteTimer > 0)
        {
            if (inputBuffer.Count > 0 && inputBuffer.Peek() == KeyCode.W)
            {
                coyoteTimer = 0;
                Jump();
                inputBuffer.Dequeue();
            }
        }
    }

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

    private void FixedUpdate()
    {
        Rigidbody2D.linearVelocity = new Vector2(Horizontal * Speed, Rigidbody2D.linearVelocity.y);
    }

    public void Die()
    {
        Animator.enabled = false;

        string nombreEscena = SceneManager.GetActiveScene().name;
        int idNivelActual = NivelMap.GetIdNivelPorNombre(nombreEscena);

        if (idNivelActual != 1)
        {
            LevelStatsManager.Instance.RegistrarMuerte();
        }
        else
        {
            Debug.Log("No se registra la muerte en el tutorial");
        }

        spriteRenderer.DOColor(Color.red, 0.0f).OnComplete(() =>
        {
            Time.timeScale = 0f;

            DOVirtual.DelayedCall(0.2f, () =>
            {
                Time.timeScale = 1f;
                DOTween.KillAll();
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }).SetUpdate(true);
        });
    }
}
