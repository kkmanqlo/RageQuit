using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public  GameObject Character; // Referencia al objeto del jugador

    // Update is called once per frame
    void Update()
    {
        Vector3 position = transform.position; // Obtener la posición actual de la cámara
        position.y = Character.transform.position.y; // Actualizar la posición x de la cámara para seguir al jugador
        transform.position = position; // Asignar la nueva posición a la cámara
    }
}
