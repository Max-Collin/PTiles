using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileClickManager : MonoBehaviour
{

    void Update()
    {
        // Detects left mouse click 
        if (Input.GetMouseButtonDown(0))
        {
            // Convert screen pixel position to 2D world coordinates
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Check if a 2D collider is under the cursor
            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

            if (hit.collider != null)
            {
                // Check if the clicked object is a Tile
                TileScript clickedTile = hit.collider.GetComponent<TileScript>();
                if (clickedTile != null)
                {
                    clickedTile.Interact();
                }
            }
        }
    }
}
