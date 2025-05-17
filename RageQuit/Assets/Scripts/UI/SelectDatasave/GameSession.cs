using UnityEngine;

public class GameSession : MonoBehaviour
{
    // Singleton para manejar la sesión actual del juego y su progreso
    // Asegura que solo exista una instancia durante toda la ejecución

    public static GameSession Instance;  // Instancia única accesible desde cualquier parte

    // Propiedades para almacenar el progreso y el slot seleccionado
    public int IdProgreso { get; set; }
    public int SlotSeleccionado { get; set; }

    void Awake()
    {
        // Si no existe instancia previa, asigna esta y marca para que no se destruya al cambiar de escena
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Si ya hay una instancia, destruye este objeto para mantener solo uno
            Destroy(gameObject);
        }
    }
}


