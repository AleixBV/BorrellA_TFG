using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Front_Trigger : MonoBehaviour {

    List<Collider> colliding = new List<Collider>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "AICarsCollider")
        {
            colliding.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "AICarsCollider")
        {
            colliding.Remove(other);
        }
    }

    public List<Collider> GetColliding()
    {
        return colliding;
    }
}
