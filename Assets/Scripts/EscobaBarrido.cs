using UnityEngine;

public class EscobaBarrido : MonoBehaviour
{
    public float velocidad = 200f;
    public float anguloMax = 45f;

    private bool barriendo = false;
    private float anguloActual = 0f;
    private int direccion = 1;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            barriendo = true;
        }

        if (barriendo)
        {
            float movimiento = velocidad * Time.deltaTime * direccion;
            transform.Rotate(Vector3.forward * movimiento);
            anguloActual += movimiento;

            if (Mathf.Abs(anguloActual) >= anguloMax)
            {
                direccion *= -1;
                anguloActual = 0f;
                barriendo = false;
            }
        }
    }
}