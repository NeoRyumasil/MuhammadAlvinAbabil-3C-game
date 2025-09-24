using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement playerMovement = other.GetComponentInParent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.Respawn();
                Debug.Log("Player Respawned");
            }
        }
    }

}
