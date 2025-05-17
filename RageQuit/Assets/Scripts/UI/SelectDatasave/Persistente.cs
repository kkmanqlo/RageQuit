using UnityEngine;

public class Persistente : MonoBehaviour
{
    // Este script se encarga de mantener el objeto al que está adjunto a través de diferentes escenas.
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

}
