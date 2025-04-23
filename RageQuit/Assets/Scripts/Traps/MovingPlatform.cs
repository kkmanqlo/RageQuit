using UnityEngine;
using DG.Tweening;

public class MovingPlatform : MonoBehaviour
{
    [Header("Movimiento Vertical")]
    public bool enableVerticalMovement = false;
    public float verticalDistance = 2f;
    public float verticalDuration = 2f;

    [Header("Movimiento Horizontal")]
    public bool enableHorizontalMovement = false;
    public float horizontalDistance = 3f;
    public float horizontalDuration = 2f;

    [Header("Agitación Idle")]
    public bool enableIdleFloat = false;
    public float floatAmplitude = 0.2f;
    public float floatDuration = 1f;

    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.position;

        if (enableVerticalMovement)
        {
            transform.DOMoveY(initialPosition.y + verticalDistance, verticalDuration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }

        if (enableHorizontalMovement)
        {
            transform.DOMoveX(initialPosition.x + horizontalDistance, horizontalDuration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }

        if (enableIdleFloat)
        {
            transform.DOMoveY(initialPosition.y + floatAmplitude, floatDuration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }
    }
}