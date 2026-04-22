using UnityEngine;

public class TriggerCasa : MonoBehaviour
{
    public AmbientManager ambientManager;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            ambientManager.EntrarCasa();
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            ambientManager.SalirCasa();
    }
}