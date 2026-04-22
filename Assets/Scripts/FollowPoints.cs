using UnityEngine;
using System.Collections.Generic;

public class FollowPoints : MonoBehaviour
{
    [Header("Puntos de animación")]
    public List<Transform> puntos = new List<Transform>();
    public float speed = 2f;
    public bool loop = true;

    private int currentIndex = 0;
    private bool animando = false;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = false;
        }
    }

    void FixedUpdate()
    {
        if (!animando || puntos.Count == 0) return;

        Transform target = puntos[currentIndex];
        Vector3 direction = (target.position - transform.position);
        float distance = direction.magnitude;

        if (distance < 0.01f)
        {
            currentIndex++;
            if (currentIndex >= puntos.Count)
            {
                if (loop) currentIndex = 0;
                else
                {
                    animando = false;
                    return;
                }
            }
        }
        else
        {
            // Movemos usando Rigidbody para respetar colisiones
            Vector3 move = direction.normalized * speed * Time.fixedDeltaTime;
            if (move.magnitude > distance) move = direction; // no sobrepasar
            rb.MovePosition(rb.position + move);
        }
    }

    public void StartAnimation()
    {
        if (puntos.Count == 0) return;
        animando = true;
        currentIndex = 0;
    }

    public void StopAnimation()
    {
        animando = false;
    }

    public bool IsAnimating()
    {
        return animando;
    }
}