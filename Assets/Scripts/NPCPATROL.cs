using UnityEngine;
using UnityEngine.AI;

public class NPCPatrol : MonoBehaviour
{
    public Transform[] puntos;   // puntos a donde caminar
    private int destinoActual = 0;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        IrSiguientePunto();
    }

    void Update()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            IrSiguientePunto();
        }
    }

    void IrSiguientePunto()
    {
        if (puntos.Length == 0)
            return;

        agent.destination = puntos[destinoActual].position;
        destinoActual = (destinoActual + 1) % puntos.Length;
    }
}