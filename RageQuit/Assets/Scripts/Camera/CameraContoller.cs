using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraContoller : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
 public float targetAspect = 16f / 9f; // Aspect ratio de diseño (ajústalo según tu nivel)
    private Camera cam;
    private int screenWidth, screenHeight;

    void Awake()
    {
        cam = GetComponent<Camera>();
        UpdateViewport();
    }

    void Update()
    {
        // Solo actualiza si cambia la resolución
        if (screenWidth != Screen.width || screenHeight != Screen.height)
        {
            UpdateViewport();
        }
    }

    void UpdateViewport()
    {
        screenWidth = Screen.width;
        screenHeight = Screen.height;

        float windowAspect = (float)screenWidth / (float)screenHeight;
        float scaleHeight = windowAspect / targetAspect;

        if (scaleHeight < 1.0f)
        {
            // Letterbox: bandas negras arriba y abajo
            Rect rect = new Rect(0, (1.0f - scaleHeight) / 2.0f, 1.0f, scaleHeight);
            cam.rect = rect;
        }
        else
        {
            // Pillarbox: bandas negras a los lados
            float scaleWidth = 1.0f / scaleHeight;
            Rect rect = new Rect((1.0f - scaleWidth) / 2.0f, 0, scaleWidth, 1.0f);
            cam.rect = rect;
        }
    }
}

