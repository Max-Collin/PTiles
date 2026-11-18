using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TileEndZone : MonoBehaviour
{
   
    //Detects if a tile leaves the screen before being interacted with
    void OnTriggerEnter2D(Collider2D other) {
        // 'other' is the collider that entered this trigger
        if (other.CompareTag("Tile"))
        {
            TileScript Tile = other.GetComponent<TileScript>();
            if (Tile.interacted)
            {
                Debug.Log("Tile entered the hit zone: " + other.name); 
            }
            
        }
    }
}
