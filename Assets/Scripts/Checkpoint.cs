using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement playerMovement = other.GetComponentInParent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement._resetPointPosition = this.transform;
                Debug.Log("Checkpoint reached!");
            }
        }
    }

}
