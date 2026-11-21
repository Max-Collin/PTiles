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
            if (!Tile.interacted)
            {
                Debug.Log("Missed Tile  -100 score"); 
                TileGameManager.Instance.ChangeScore(-100);
                TileGameManager.Instance.MissedTile( Tile);
            }

            if (!Tile.GetTileData().isHeld)
            {
                Destroy(other.gameObject);
            }

            if ((Tile.GetTileData().isHeld && Tile.hasCompletedHold) || (Tile.GetTileData().isHeld && !Tile.interacted))
            {
                Destroy(other.gameObject);
            }
            
        }
    }
}
