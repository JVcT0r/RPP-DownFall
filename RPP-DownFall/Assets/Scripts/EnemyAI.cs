using Pathfinding;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    void Start()
    {
        var player = GameObject.FindGameObjectWithTag("Player")?.transform;
        GetComponent<AIDestinationSetter>().target = player;
    }
}

