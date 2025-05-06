using UnityEngine;

public class GameSession : MonoBehaviour
{
    public static GameSession Instance;

    public int IdProgreso { get; set; }
    public int SlotSeleccionado { get; set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

