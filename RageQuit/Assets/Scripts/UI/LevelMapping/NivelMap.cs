using System.Collections.Generic;
using UnityEngine;

public static class NivelMap
{
    public static List<NivelData> Niveles = new List<NivelData>
    {
        new NivelData { idNivel = 1, nombreEscena = "Tutorial" },
        new NivelData { idNivel = 2, nombreEscena = "Nivel 1" },
        new NivelData { idNivel = 3, nombreEscena = "Nivel 2" },
        new NivelData { idNivel = 4, nombreEscena = "Nivel 3" },
        new NivelData { idNivel = 5, nombreEscena = "Nivel 4" },
        new NivelData { idNivel = 6, nombreEscena = "Nivel 5" },
        new NivelData { idNivel = 7, nombreEscena = "Nivel 6" },
        new NivelData { idNivel = 8, nombreEscena = "Nivel 7" },
    };

    public static int GetIdNivelPorNombre(string nombreEscena)
    {
        var nivel = Niveles.Find(n => n.nombreEscena == nombreEscena);
        return nivel != null ? nivel.idNivel : -1;
    }
}
