using System.Collections.Generic;
using UnityEngine;

public static class NivelMap
{
    // Lista estática de niveles con su ID y nombre de escena
    public static List<NivelData> Niveles = new List<NivelData>
    {
        new NivelData { idNivel = 1, nombreEscena = "Tutorial" },
        new NivelData { idNivel = 2, nombreEscena = "Level 1" },
        new NivelData { idNivel = 3, nombreEscena = "Level 2" },
        new NivelData { idNivel = 4, nombreEscena = "Level 3" },
        new NivelData { idNivel = 5, nombreEscena = "Level 4" },
        new NivelData { idNivel = 6, nombreEscena = "Level 5" },
    };

    // Método para obtener el ID del nivel a partir del nombre de la escena
    public static int GetIdNivelPorNombre(string nombreEscena)
    {
        var nivel = Niveles.Find(n => n.nombreEscena == nombreEscena);
        return nivel != null ? nivel.idNivel : -1; // Retorna -1 si no encuentra el nivel
    }
}
