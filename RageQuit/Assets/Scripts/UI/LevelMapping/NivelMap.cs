using System.Collections.Generic;
using UnityEngine;

public static class NivelMap
{
    public static List<NivelData> Niveles = new List<NivelData>
    {
        new NivelData { idNivel = 1, nombreEscena = "Tutorial" },
        new NivelData { idNivel = 2, nombreEscena = "Level 1" },
        new NivelData { idNivel = 3, nombreEscena = "Level 2" },
        new NivelData { idNivel = 4, nombreEscena = "Level 3" },
        new NivelData { idNivel = 5, nombreEscena = "Level 4" },
        new NivelData { idNivel = 6, nombreEscena = "Level 5" },
    };

    public static int GetIdNivelPorNombre(string nombreEscena)
    {
        var nivel = Niveles.Find(n => n.nombreEscena == nombreEscena);
        return nivel != null ? nivel.idNivel : -1;
    }
}
