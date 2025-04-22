using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public  GameObject Character; // Referencia al objeto del jugador

    // Update is called once per frame
    void Update()
    {
        Vector3 positionY = transform.position; // Obtener la posición actual de la cámara
        positionY.y = Character.transform.position.y; // Actualizar la posición y de la cámara para seguir al jugador
        transform.position = positionY; // Asignar la nueva posición a la cámara

        Vector3 positionX = transform.position; // Obtener la posición actual de la cámara
        positionX.x = Character.transform.position.x; // Actualizar la posición x de la cámara para seguir al jugador
        transform.position = positionX; // Asignar la nueva posición a la cámara
    }
}
