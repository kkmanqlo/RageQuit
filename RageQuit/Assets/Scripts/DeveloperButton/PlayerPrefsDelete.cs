using UnityEngine;

public class PlayerPrefsDelete : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
public void BorrarPrefs()
{
    PlayerPrefs.DeleteAll();
    Debug.Log("PlayerPrefs borrados.");
}
}
