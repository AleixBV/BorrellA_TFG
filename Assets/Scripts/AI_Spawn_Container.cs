using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Spawn_Container : MonoBehaviour {

    [SerializeField] AI_Intersection[] Spawns;

    public AI_Intersection GetRandomSpawner()
    {
        return Spawns[Random.Range(0, Spawns.Length)];
    }
}
