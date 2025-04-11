using UnityEngine;

public class ControladorMenu : MonoBehaviour
{
    public GameObject PopUp;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MostrarPopUp()
    {
        PopUp.SetActive(true);
    }

    public void OcultarPopUp()
    {
        PopUp.SetActive(false);
    }
}
