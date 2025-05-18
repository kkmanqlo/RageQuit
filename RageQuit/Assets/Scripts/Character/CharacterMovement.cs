using System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem.LowLevel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class CharacterMovement : MonoBehaviour
{
    // Velocidad horizontal del personaje.
    public float Speed;

    // Fuerza del salto.
    public float JumpForce;

    // Duración del "coyote time", que permite saltar poco después de haber salido del suelo.
    private float coyoteTime = 0.11f;
    private float coyoteTimer;

    // Indica si ya se ha hecho buffer de un salto.
    private bool hasJumpBuffered = false;

    // Referencia al Rigidbody2D para controlar la física.
    private Rigidbody2D Rigidbody2D;

    // Cola para guardar inputs (buffer de salto).
    private Queue<KeyCode> inputBuffer;

    // Referencia al Animator para controlar animaciones.
    private Animator Animator;

    // Dirección horizontal del movimiento.
    private float Horizontal;

    // Indica si el personaje está tocando el suelo.
    private bool Grounded;

    // Referencia al SpriteRenderer (para cambiar color al morir).
    private SpriteRenderer spriteRenderer;

    // Color original del sprite (para restaurarlo si es necesario).
    private Color originalColor;

    // Resultado del raycast que detecta el suelo.
    public RaycastHit2D hit;

    // Capa que se considera "suelo" para los raycasts.
    public LayerMask groundLayer;

    // Ajuste vertical del origen del raycast (altura desde donde se lanza).
    private float offsetY = 0f;

    void Start()
    {
        // Obtener componentes necesarios.
        Rigidbody2D = GetComponent<Rigidbody2D>();
        Rigidbody2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        Animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;

        // Inicializar el buffer de inputs.
        inputBuffer = new Queue<KeyCode>();
    }

    void Update()
    {
        if (GameManager.Instance.inputBloqueado) return;
        // Leer input horizontal (teclas A/D o flechas).
        Horizontal = Input.GetAxisRaw("Horizontal");

        // Girar el sprite hacia la izquierda o derecha según dirección.
        if (Horizontal < 0.0f)
            transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        else if (Horizontal > 0.0f)
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        // Actualizar parámetros del Animator para reflejar el estado.
        Animator.SetBool("grounded", Grounded);
        Animator.SetBool("running", Grounded && Horizontal != 0.0f);
        Animator.SetBool("falling", !Grounded && Rigidbody2D.linearVelocity.y < 0.0f);
        Animator.SetFloat("yvelocity", Rigidbody2D.linearVelocity.y);
        Animator.SetFloat("xvelocity", Rigidbody2D.linearVelocity.x);

        // Determinar origen y distancia del raycast hacia abajo (para saber si estamos en el suelo).
        Vector2 origenRaycast = (Vector2)transform.position + Vector2.down * offsetY;
        float distanciaRaycast = 0.15f;

        // Lanzar raycast hacia abajo para detectar suelo.
        hit = Physics2D.Raycast(origenRaycast, Vector2.down, distanciaRaycast, groundLayer);
        Debug.DrawRay(origenRaycast, Vector2.down * distanciaRaycast, hit.collider != null ? Color.green : Color.red);

        if (hit.collider != null)
        {
            // Si toca suelo, activar grounded y reiniciar coyote timer.
            Grounded = true;
            coyoteTimer = coyoteTime;
        }
        else
        {
            // Si no toca suelo, disminuir el coyote timer.
            Grounded = false;
            coyoteTimer -= Time.deltaTime;
        }

        // Si el jugador presiona W y no hay ya un salto en buffer, guardarlo.
        if (Input.GetKeyDown(KeyCode.W) && inputBuffer.Count == 0 && !hasJumpBuffered)
        {
            inputBuffer.Enqueue(KeyCode.W);
            hasJumpBuffered = true;
            Invoke("quitarAccion", 0.3f); // Después de 0.3s eliminamos el buffer si no se usó.
        }

        // Si aún estamos dentro del coyote time y hay un salto en buffer, ejecutar salto.
        if (coyoteTimer > 0)
        {
            if (inputBuffer.Count > 0 && inputBuffer.Peek() == KeyCode.W)
            {
                coyoteTimer = 0; // Anulamos el coyote time para evitar dobles saltos.
                Jump();
                inputBuffer.Dequeue(); // Eliminamos el input usado.
            }
        }
    }

    // Método para ejecutar el salto.
    private void Jump()
    {
        // Reiniciar velocidad vertical antes de aplicar fuerza de salto.
        Rigidbody2D.linearVelocity = new Vector2(Rigidbody2D.linearVelocity.x, 0);
        Rigidbody2D.AddForce(Vector2.up * JumpForce);
        Animator.SetTrigger("jump"); // Activar animación de salto.
    }

    // Método llamado tras un breve delay para eliminar el input buffered si no se usó.
    void quitarAccion()
    {
        if (inputBuffer.Count > 0)
            inputBuffer.Dequeue();
        hasJumpBuffered = false;
    }

    // Física se maneja en FixedUpdate.
    private void FixedUpdate()
    {
        // Aplicar movimiento horizontal (manteniendo velocidad vertical).
        Rigidbody2D.linearVelocity = new Vector2(Horizontal * Speed, Rigidbody2D.linearVelocity.y);
    }

    // Método que se llama cuando el personaje muere.
    public void Die()
    {
        // Desactivar animaciones.
        Animator.enabled = false;

        // Obtener nombre de la escena actual para registrar la muerte correctamente.
        string nombreEscena = SceneManager.GetActiveScene().name;
        int idNivelActual = NivelMap.GetIdNivelPorNombre(nombreEscena);

        // Registrar muerte si no es el tutorial.
        if (idNivelActual != 1)
        {
            LevelStatsManager.Instance.RegistrarMuerte();
        }
        else
        {
            Debug.Log("No se registra la muerte en el tutorial");
        }

        // Cambiar el color del personaje a rojo instantáneamente.
        spriteRenderer.DOColor(Color.red, 0.0f).OnComplete(() =>
        {
            // Pausar el tiempo para crear un pequeño efecto visual.
            Time.timeScale = 0f;

            // Después de 0.2 segundos (reales, no afectados por Time.timeScale):
            DOVirtual.DelayedCall(0.2f, () =>
            {
                Time.timeScale = 1f; // Volver a activar el tiempo.
                DOTween.KillAll();   // Cancelar animaciones pendientes.
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Reiniciar escena.
            }).SetUpdate(true); // Se asegura de que el delay ocurra aunque el juego esté pausado.
        });
    }
}

